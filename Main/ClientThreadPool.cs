using Arcam.Data.DataBase;
using Arcam.Data.DataBase.DBTypes;
using Arcam.Indicators;
using Arcam.Indicators.IndicatorsSerealizers;
using Arcam.Main.Loggers;
using Arcam.Market;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Collections.Immutable;

namespace Arcam.Main
{
    public class ClientThreadPool
    {
        static object locker = new object();
        static List<string> threadNames = new List<string>();
        static Dictionary<string, DateTime> lastResponse = new Dictionary<string, DateTime>();
        Dictionary<string, Task> threads = new Dictionary<string, Task>();
        Dictionary<string, CancellationTokenSource> cancellationToken;
        List<string> names = new List<string>();
        Logger logger = new Logger(typeof(ClientThreadPool));
        Type workerType;
        public ClientThreadPool(Type workerType, Type platformType)
        {
            this.workerType = workerType;
            cancellationToken = new Dictionary<string, CancellationTokenSource>();
                using (ApplicationContext db = new ApplicationContext())
            {
                var WorkingPair = db.WorkingPair.ToList();
                var users = db.User.ToList();
                var InputField = db.InputField.ToList();
                var accountsAll = db.Account.ToList();
                    foreach (var eachAcc in accountsAll)
                    {
                        db.Entry(eachAcc).Reference(x => x.User).Load();
                        var each = eachAcc.User;
                        db.Entry(eachAcc).Reference(x => x.Strategy).Load();
                        db.Entry(eachAcc).Reference(x => x.Platform).Load();
                        //Type platformType = Type.GetType(eachAcc.Platform.ClassName);
                        var platformConstructor = platformType.GetConstructor(new Type[] { typeof(string), typeof(string), typeof(string) });
                        if (platformConstructor == null)
                            throw new Exception("No constructor for Platform " + platformType.Name);
                        IPlatform platform = (IPlatform)platformConstructor.Invoke(new object[] { eachAcc.Platform.Url, eachAcc.key, eachAcc.secret });
                        PairSetting setting = new PairSetting();
                        var strategy = eachAcc.Strategy;
                        db.Entry(strategy).Reference(x => x.Timing).Load();
                        db.Entry(strategy).Reference(x => x.Pair).Load();
                        setting.timeSpan = strategy.Timing.Value;
                        setting.canLong = strategy.IsLong > 0;
                        setting.canShort = strategy.IsShort > 0;
                        setting.leverage = strategy.Leverage;
                        setting.indicators = new List<IndicatorParams>();
                        var indicators = db.StrategyIndicator.Where(x => x.Strategy == strategy).ToList();
                        foreach (var indicator in indicators)
                        {
                            var param = new IndicatorParams();
                            db.Entry(indicator).Reference(x => x.Indicator).Load();
                            var fields = db.InputField.Where(x => x.StrategyIndicatorId == indicator.Id).ToList();
                            foreach (var field in fields)
                            {
                                db.Entry(field).Reference(x => x.IndicatorField).Load();
                                param.parameters[field.IndicatorField.Name] = field.IntValue.Value;
                            }
                            param.isExit = indicator.IsExit > 0;
                            param.classname = indicator.Indicator.ClassName;
                            param.name = indicator.Indicator.Name;
                            setting.indicators.Add(param);
                        }
                        setting.pair = strategy.Pair.Name;
                        var workerConstructor = workerType.GetConstructor(new Type[] { typeof(IPlatform), typeof(PairSetting) });
                        if (workerConstructor == null)
                            throw new Exception("No constructor for Worker " + workerType.Name);
                        var tokenSource = new CancellationTokenSource();
                        threadNames.Add(each.Name);
                        var worker = (Worker)workerConstructor.Invoke(new object[] { platform, setting });
                        var thread = new Task(() =>
                        {
                            Thread.CurrentThread.Name = each.Name;
                            try
                            {
                                worker.Start();
                            }
                            catch (OperationCanceledException)
                            {
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex);
                            }
                        }, tokenSource.Token);
                        lastResponse.Add(each.Name, DateTime.Now);
                        cancellationToken.Add(each.Name, tokenSource);
                        threads.Add(each.Name, thread);
                        logger.Info("Started thread " + threadNames[^1]); ;
                    }
                }
            
        }

        void CheckAndRestartThreads()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var wp = db.WorkingPair.Find(2L);
                Console.WriteLine(wp.Name);
                db.Entry(wp).Reference(x => x.platform).Load();
                Console.WriteLine(wp.platform.Name);

                var users = db.User.ToList();
                foreach (var each in users)
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
                        }
                        else
                            continue;
                    }
                    var accounts = db.Account.Where(x => x.User.Id == each.Id).ToList();
                    foreach (var eachAcc in accounts)
                    {
                        db.Entry(eachAcc).Reference(x => x.Strategy).Load();
                        db.Entry(eachAcc).Reference(x => x.Platform).Load();
                        Type platformType = Type.GetType(eachAcc.Platform.ClassName);
                        var platformConstructor = platformType.GetConstructor(new Type[] { typeof(string), typeof(string), typeof(string) });
                        if (platformConstructor == null)
                            throw new Exception("No constructor for Platform " + platformType.Name);
                        IPlatform platform = (IPlatform)platformConstructor.Invoke(new object[] { eachAcc.Platform.Url, eachAcc.key, eachAcc.secret });
                        PairSetting setting = new PairSetting();
                        var strategy = eachAcc.Strategy;
                        db.Entry(strategy).Reference(x => x.Timing).Load();
                        db.Entry(strategy).Reference(x => x.Pair).Load();
                        setting.timeSpan = strategy.Timing.Value;
                        setting.canLong = strategy.IsLong > 0;
                        setting.canShort = strategy.IsShort > 0;
                        setting.leverage = strategy.Leverage;
                        setting.indicators = new List<IndicatorParams>();
                        var indicators = db.StrategyIndicator.Where(x => x.Strategy == strategy).ToList();
                        foreach (var indicator in indicators)
                        {
                            var param = new IndicatorParams();
                            db.Entry(indicator).Reference(x => x.Indicator).Load();
                            var fields = db.InputField.Where(x => x.StrategyIndicator == indicator).ToList();
                            foreach (var field in fields)
                            {
                                db.Entry(field).Reference(x => x.IndicatorField).Load();
                                param.parameters[field.IndicatorField.Name] = field.IntValue.Value;
                            }
                            param.isExit = indicator.IsExit > 0;
                            param.classname = indicator.Indicator.ClassName;
                            param.name = indicator.Indicator.Name;
                            setting.indicators.Add(param);
                        }
                        var workerConstructor = workerType.GetConstructor(new Type[] { typeof(IPlatform), typeof(PairSetting) });
                        if (workerConstructor == null)
                            throw new Exception("No constructor for Worker " + workerType.Name);
                        var tokenSource = new CancellationTokenSource();
                        threadNames.Add(strategy.Name);
                        var worker = (Worker)workerConstructor.Invoke(new object[] { platform, setting });
                        var thread = new Task(() =>
                        {
                            Thread.CurrentThread.Name = each.Name;
                            try
                            {
                                worker.Start();
                            }
                            catch (OperationCanceledException)
                            {
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex);
                            }
                        }, tokenSource.Token);
                        lastResponse.Add(each.Name, DateTime.Now);
                        cancellationToken.Add(each.Name, tokenSource);
                        threads.Add(each.Name, thread);
                        logger.Info("Restarted thread " + threadNames[^1]); ;
                    }
                }
            }
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
