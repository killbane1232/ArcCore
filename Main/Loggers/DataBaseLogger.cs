namespace Arcam.Main.Loggers
{
    internal class DataBaseLogger : ILogger
    {
        string baseName;
        public static bool isDebug = false;
        

        public DataBaseLogger(Type ShortFileName)
        {
            baseName = ShortFileName.Name;
        }

        public DataBaseLogger(string name)
        {
            baseName = name;
        }
        public void Debug(object data)
        {
            var log = new Log();
            log.ClassName = baseName;
            log.UserId = (0);
            log.Data = data.ToString() ?? "";
            log.Type = 2;
            log.LogDate = DateTime.Now;
            using (var db = new ApplicationContext())
            {
                db.Add(log);
                db.SaveChanges();
            }
        }
        public void Debug(string symbol, object data)
        {
            var log = new Log();
            log.ClassName = baseName;
            log.UserId = (0);
            log.Data = data.ToString() ?? "";
            log.Type = 2;
            log.Addictional = "symbol:" + symbol;
            log.LogDate = DateTime.Now;
            using (var db = new ApplicationContext())
            {
                db.Add(log);
                db.SaveChanges();
            }
        }
        public void Log(object data, string type)
        {
            var log = new Log();
            log.ClassName = baseName;
            log.UserId = (0);
            log.Data = data.ToString() ?? "";
            log.Type = type == "message"? 0:1;
            log.LogDate = DateTime.Now;
            using (var db = new ApplicationContext())
            {
                db.Add(log);
                db.SaveChanges();
            }
        }
        public void Log(string symbol, object data, string type)
        {
            var log = new Log();
            log.ClassName = baseName;
            log.UserId = (0);
            log.Data = data.ToString() ?? "";
            log.Type = type == "message" ? 0 : 1;
            log.Addictional = "symbol:" + symbol;
            log.LogDate = DateTime.Now;
            using (var db = new ApplicationContext())
            {
                db.Add(log);
                db.SaveChanges();
            }
        }
    }
}
