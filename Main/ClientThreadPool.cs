using Arcam.Indicators.IndicatorsSerealizers;
using Arcam.Main.Loggers;
using Arcam.Market;

namespace Arcam.Main
{
    public class ClientThreadPool
    {
        List<Task> threads;
        List<CancellationTokenSource> cancellationToken;
        List<string> names;
        IIndicatorsSerializer sere;
        Type platformType;
        Logger logger = new Logger(typeof(ClientThreadPool));
        Type workerType;
        public ClientThreadPool(IIndicatorsSerializer sere, Type platformType, Type workerType)
        {
            this.platformType = platformType;
            this.workerType = workerType;
            this.sere = sere;
            threads = new List<Task>();
            cancellationToken = new List<CancellationTokenSource>();
            names = new List<string>();
            var directoryFiles = Directory.EnumerateFiles("./api");
            sere.LoadIndicator();
            var threadNames = new List<string>();
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
                    worker.WorkerPreparer();
                }, tokenSource.Token);
                cancellationToken.Add(tokenSource);
                threads.Add(thread);
                names.Add(file);
                threadNames.Add(info.Name);
                logger.Info("Started thread " + names[^1]);
            }
            ConsoleUI.PrepareMenu(threadNames);
        }

        void CheckAndRestartThreads()
        {
            for (var i = 0; i < cancellationToken.Count; i++)
            {
                var each = cancellationToken[i];
                var task = threads[i];
                if (task.Status == TaskStatus.Faulted || task.Status == TaskStatus.Canceled)
                {
                    each.Cancel();
                    try
                    {
                        task.Wait();
                    }
                    catch
                    {
                        //Console.WriteLine($"Error thrown with message: {e.Message}");
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
                            worker.WorkerPreparer();
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
                catch
                {
                    //Console.WriteLine($"Error thrown with message: {e.Message}");
                }
                finally
                {
                    each.Dispose();
                }
            }
        }
    }
}
