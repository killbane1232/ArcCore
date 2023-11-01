using Arcam.Indicators.IndicatorsSerealizers;
using Arcam.Main.Loggers;
using Arcam.Market;

namespace Arcam.Main
{
    public class ClientThreadPool
    {
        static object locker = new object();
        static List<string> threadNames = new List<string>();
        static List<DateTime> lastResponse = new List<DateTime>();
        List<Task> threads = new List<Task>();
        List<CancellationTokenSource> cancellationToken;
        List<string> names = new List<string>();
        IIndicatorsSerializer sere;
        Type platformType;
        Logger logger = new Logger(typeof(ClientThreadPool));
        Type workerType;
        public ClientThreadPool(IIndicatorsSerializer sere, Type platformType, Type workerType)
        {
            this.platformType = platformType;
            this.workerType = workerType;
            this.sere = sere;
            cancellationToken = new List<CancellationTokenSource>();
            var directoryFiles = Directory.EnumerateFiles("./api");
            sere.LoadIndicator();
            foreach (var file in directoryFiles)
            {
                if (!File.Exists(file))
                    continue;
                var info = new FileInfo(file);
                var lines = File.ReadAllLines(file);
                if (lines.Length < 2 || lines[0].Length < 24 || lines[1].Length < 48)
                    continue;
                var platformConstructor = platformType.GetConstructor(new Type[] { typeof(string), typeof(string) });
                if (platformConstructor == null)
                    throw new Exception("No constructor for Platform " + platformType.Name);
                IPlatform platform = (IPlatform)platformConstructor.Invoke(new object[] { lines[0], lines[1] });
                var tokenSource = new CancellationTokenSource();
                var workerConstructor = workerType.GetConstructor(new Type[] { typeof(IPlatform), typeof(CancellationToken), typeof(IIndicatorsSerializer) });
                if (workerConstructor == null)
                    throw new Exception("No constructor for Worker " + workerType.Name);
                var worker = (Worker)workerConstructor.Invoke(new object[] { platform, tokenSource.Token, sere });
                var thread = new Task(() =>
                {
                    Thread.CurrentThread.Name = info.Name;
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
                lastResponse.Add(DateTime.Now);
                cancellationToken.Add(tokenSource);
                threads.Add(thread);
                names.Add(file);
                threadNames.Add(info.Name);
                logger.Info("Started thread " + threadNames[^1]);;
            }
            ConsoleUI.PrepareMenu(threadNames);
        }

        void CheckAndRestartThreads()
        {
            for (var i = 0; i < cancellationToken.Count; i++)
            {
                var each = cancellationToken[i];
                var task = threads[i];
                if (task.Status == TaskStatus.Faulted || task.Status == TaskStatus.RanToCompletion || task.Status == TaskStatus.Canceled || (DateTime.Now - lastResponse[i]).TotalMinutes > 1)
                {
                    each.Cancel();
                    try
                    {
                        task.Wait();
                    }
                    catch
                    {
                    }
                    finally
                    {
                        each.Dispose();
                        var info = new FileInfo(names[i]);
                        var lines = File.ReadAllLines(names[i]);
                        var platformConstructor = platformType.GetConstructor(new Type[] { typeof(string), typeof(string) });
                        if (platformConstructor == null)
                            throw new Exception("No constructor for Platform " + platformType.Name);
                        IPlatform platform = (IPlatform)platformConstructor.Invoke(new object[] { lines[0], lines[1] });
                        var tokenSource = new CancellationTokenSource();
                        var constructor = workerType.GetConstructor(new Type[] { typeof(IPlatform), typeof(CancellationToken), typeof(int), typeof(IIndicatorsSerializer) });
                        if (constructor == null)
                            throw new Exception("No constructor for Worker " + workerType.Name);
                        var worker = (Worker)constructor.Invoke(new object[] { platform, tokenSource.Token, threads.Count, sere });
                        task = new Task(() =>
                        {
                            Thread.CurrentThread.Name = info.Name;
                            worker.Start();
                        }, tokenSource.Token);
                        cancellationToken[i] = tokenSource;
                        task.Start();
                        threads[i] = task;
                        logger.Info("Restarted thread " + names[i]);
                    }
                }
            }
        }
        public void Start()
        {
            foreach (var task in threads)
                task.Start();
            while (true)
            {
                Thread.Sleep(30000);
                CheckAndRestartThreads();
            }
        }
        public void Stop()
        {
            for (var i = 0; i < cancellationToken.Count; i++)
            {
                var each = cancellationToken[i];
                var task = threads[i];
                each.Cancel();
                try
                {
                    task.Wait();
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
                var i = threadNames.FindIndex(x => x == Thread.CurrentThread.Name);
                lastResponse[i] = DateTime.Now;
            }
        }
    }
}
