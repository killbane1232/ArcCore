using Arcam.Data.DataBase;
using Arcam.Data.DataBase.DBTypes;
using Arcam.Market;

namespace Arcam.Main
{
    public class ClientThreadPool
    {
        private static readonly object locker = new();
        private readonly static Dictionary<string, StartedThread> threads = [];
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        readonly Type workerType;
        readonly Type platformType;
        public ClientThreadPool(Type workerType, Type platformType)
        {
            this.workerType = workerType;
            this.platformType = platformType;
            logger.Info("Starting threads");
            using ApplicationContext db = new();
            var accountsAll = db.Account.Where(x => x.IsActive == true).ToList();
            foreach (var eachAcc in accountsAll)
            {
                StartThread(eachAcc, db);
                logger.Info("Started thread " + eachAcc.Name);
            }
        }

        void CheckAndRestartThreads()
        {
            using ApplicationContext db = new ApplicationContext();
            var accounts = db.Account.Where(x => x.IsActive == true).ToList();
            foreach (var each in threads.Keys)
            {
                if (!accounts.Any(x => x.Name == each))
                {
                    var thread = threads[each];
                    thread.ct!.Cancel();
                    try
                    {
                        threads[each].task!.Wait();
                    }
                    catch
                    {
                    }
                    finally
                    {
                        thread.ct.Dispose();
                    }
                    threads.Remove(each);
                    logger.Info("Stopped thread " + each);
                }
            }
            foreach (var each in accounts)
            {
                if (threads.TryGetValue(each.Name, out StartedThread? thread))
                {
                    var strategy = GetStrategy(each, db);
                    if (thread.task!.Status == TaskStatus.Faulted ||
                        thread.task.Status == TaskStatus.RanToCompletion ||
                        thread.task.Status == TaskStatus.Canceled ||
                        (DateTime.Now - thread.lastResponse).TotalMinutes > 1 ||
                        strategy.GetHashCode() != thread.stratHash)
                    {
                        thread.ct!.Cancel();
                        try
                        {
                            thread.task.Wait();
                        }
                        catch { }
                        finally
                        {
                            thread.ct.Dispose();
                        }
                        logger.Info("Stopped thread " + each.Name);
                    }
                    else
                        continue;
                }
                StartThread(each, db);
                threads[each.Name].task!.Start();
                logger.Info("Restarted thread " + each.Name);
            }
        }

        void StartThread(Account account, ApplicationContext db)
        {
            db.Entry(account).Reference(x => x.Platform).Load();
            var platformConstructor = platformType.GetConstructor([typeof(string), typeof(string), typeof(string)]) ??
                throw new Exception("No constructor for Platform " + platformType.Name);
            IPlatform platform = (IPlatform)platformConstructor.Invoke(new object[] { account.Platform.Url, account.Key, account.Secret });
            var strategy = GetStrategy(account, db);

            var workerConstructor = workerType.GetConstructor([typeof(IPlatform), typeof(Strategy), typeof(Account)]) ??
                throw new Exception("No constructor for Worker " + workerType.Name);
            var tokenSource = new CancellationTokenSource();
            var worker = (Worker)workerConstructor.Invoke(new object[] { platform, strategy, account });
            var thread = new Task(() =>
            {
                try
                {
                    worker.ct = tokenSource.Token;
                    Thread.CurrentThread.Name = account.Name;
                    worker.Start();
                }
                catch (OperationCanceledException ex)
                {
                    logger.Error(ex);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }, tokenSource.Token);
            threads[account.Name] = new StartedThread()
            {
                lastResponse = DateTime.Now,
                ct = tokenSource,
                stratHash = strategy.GetHashCode(),
                task = thread
            };
        }

        static Strategy GetStrategy(Account account, ApplicationContext db)
        {
            db.Entry(account).Reference(x => x.Strategy).Load();
            var strategy = account.Strategy!;
            db.Entry(strategy).Reference(x => x.Timing).Load();
            db.Entry(strategy).Reference(x => x.Pair).Load();
            db.Entry(strategy).Collection(x => x.StrategyIndicators).Load();

            foreach (var indicator in strategy.StrategyIndicators)
            {
                indicator.Strategy = strategy;
                db.Entry(indicator).Reference(x => x.Indicator).Load();
                var fields = db.InputField.Where(x => x.StrategyIndicatorId == indicator.Id).ToList();
                foreach (var field in fields)
                {
                    db.Entry(field).Reference(x => x.IndicatorField).Load();
                    indicator.InputFields[field.IndicatorField.CodeName!] = field;
                }
            }
            return strategy;
        }

        public void Start()
        {
            foreach (var task in threads)
                task.Value.task!.Start();
            while (true)
            {
                Thread.Sleep(30000);
                CheckAndRestartThreads();
            }
        }
        public void Stop()
        {
            foreach (var thread in threads)
            {
                thread.Value.ct!.Cancel();
                try
                {
                    thread.Value.task!.Wait();
                }
                catch (OperationCanceledException) { }
                catch (AggregateException ex)
                {
                    ex.Handle(ex =>
                    {
                        logger.Error(ex);
                        return true;
                    });
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
                finally
                {
                    try
                    {
                        thread.Value.ct.Dispose();
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }
                }
            }
        }
        public static void SetLastResponse()
        {
            lock (locker)
            {
                threads[Thread.CurrentThread.Name ?? ""].lastResponse = DateTime.Now;
            }
        }
        private class StartedThread
        {
            public Task? task;
            public string? stratHash;
            public CancellationTokenSource? ct;
            public DateTime lastResponse;
        }
    }
}
