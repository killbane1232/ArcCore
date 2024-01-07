using Arcam.Data.DataBase;
using Arcam.Data.DataBase.DBTypes;
using Arcam.Market;
using Microsoft.EntityFrameworkCore;
namespace Arcam.Main
{
    public class ClientThreadPool
    {
        private static object locker = new();
        static List<string> threadNames = new List<string>();
        static Dictionary<string, DateTime> lastResponse = new Dictionary<string, DateTime>();
        private Dictionary<string, Task> threads = new Dictionary<string, Task>();
        private Dictionary<string, CancellationTokenSource> cancellationToken;
        private Logger logger = LogManager.GetCurrentClassLogger();
        Type workerType;
        Type platformType;
        public ClientThreadPool(Type workerType, Type platformType)
        {
            this.workerType = workerType;
            this.platformType = platformType;
            cancellationToken = new Dictionary<string, CancellationTokenSource>();
            logger.Info("Starting threads");
            using (ApplicationContext db = new ApplicationContext())
            {
                var accountsAll = db.Account.Where(x => x.IsActive == true).ToList();
                foreach (var eachAcc in accountsAll)
                {
                    StartThread(eachAcc, db);
                    logger.Info("Started thread " + threadNames[^1]);
                }
            }
        }

        void CheckAndRestartThreads()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var accounts = db.Account.Where(x => x.IsActive == true).ToList();
                foreach (var each in cancellationToken.Keys)
                {
                    if (!accounts.Any(x => x.Name == each))
                    {
                        cancellationToken[each].Cancel();
                        try
                        {
                            threads[each].Wait();
                        }
                        catch
                        {
                        }
                        finally
                        {
                            cancellationToken[each].Dispose();
                        }
                        cancellationToken.Remove(each);
                        logger.Info("Stopped thread " + each);
                    }
                }
                foreach (var each in accounts)
                {
                    if (cancellationToken.ContainsKey(each.Name) && threads.ContainsKey(each.Name))
                    {
                        var ct = cancellationToken[each.Name];
                        var task = threads[each.Name];
                        if (task.Status == TaskStatus.Faulted || task.Status == TaskStatus.RanToCompletion || task.Status == TaskStatus.Canceled || (DateTime.Now - lastResponse[each.Name]).TotalMinutes > 1)
                        {
                            ct.Cancel();
                            try
                            {
                                task.Wait();
                            }
                            catch
                            {
                            }
                            finally
                            {
                                ct.Dispose();
                            }
                            logger.Info("Stopped thread " + each.Name);
                        }
                        else
                            continue;
                    }
                    StartThread(each, db);
                    threads[each.Name].Start();
                    logger.Info("Restarted thread " + each.Name);
                }
            }
        }

        void StartThread(Account account, ApplicationContext db)
        {
            db.Entry(account).Reference(x => x.Strategy).Load();
            db.Entry(account).Reference(x => x.Platform).Load();
            //Type platformType = Type.GetType(eachAcc.Platform.ClassName);
            var platformConstructor = platformType.GetConstructor(new Type[] { typeof(string), typeof(string), typeof(string) });
            if (platformConstructor == null)
                throw new Exception("No constructor for Platform " + platformType.Name);
            IPlatform platform = (IPlatform)platformConstructor.Invoke(new object[] { account.Platform.Url, account.Key, account.Secret });
            var strategy = account.Strategy;
            db.Entry(strategy).Reference(x => x.Timing).Load();
            db.Entry(strategy).Reference(x => x.Pair).Load();
            db.Entry(strategy).Collection(x => x.StrategyIndicators).Load();

            foreach (var indicator in strategy.StrategyIndicators)
            {
                db.Entry(indicator).Reference(x => x.Indicator).Load();
                var fields = db.InputField.Where(x => x.StrategyIndicatorId == indicator.Id).ToList();
                foreach (var field in fields)
                {
                    db.Entry(field).Reference(x => x.IndicatorField).Load();
                    indicator.InputFields.Add(field.IndicatorField.CodeName, field);
                }
            }

            var workerConstructor = workerType.GetConstructor(new Type[] { typeof(IPlatform), typeof(Strategy) });
            if (workerConstructor == null)
                throw new Exception("No constructor for Worker " + workerType.Name);
            var tokenSource = new CancellationTokenSource();
            threadNames.Add(account.Name);
            var worker = (Worker)workerConstructor.Invoke(new object[] { platform, strategy });
            var thread = new Task(() =>
            {
                try
                {
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
            lastResponse[account.Name] = DateTime.Now;
            cancellationToken[account.Name] = tokenSource;
            threads[account.Name] = thread;
        }
        public void Start()
        {
            foreach (var task in threads)
                task.Value.Start();
            while (true)
            {
                Thread.Sleep(30000);
                CheckAndRestartThreads();
            }
        }
        public void Stop()
        {
            foreach (var task in threads)
            {
                var each = cancellationToken[task.Key];
                each.Cancel();
                try
                {
                    task.Value.Wait();
                }
                catch (OperationCanceledException)
                {
                }
                catch (AggregateException ex)
                {
                    ex.Handle(ex => {
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
                        each.Dispose();
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
                lastResponse[Thread.CurrentThread.Name??""] = DateTime.Now;
            }
        }
    }
}
