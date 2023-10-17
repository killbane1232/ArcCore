using Microsoft.EntityFrameworkCore;

namespace Arcam.Main.Loggers
{
    public class Log
    {
        [BackingField("id")]
        public int Id { get; set; }
        [BackingField("type")]
        public int Type { get; set; }
        [BackingField("data")]
        public string Data { get; set; } = "";
        [BackingField("user_id")]
        public int UserId { get; set; }
        [BackingField("class_name")]
        public string? ClassName { get; set; }
        [BackingField("addictional")]
        public string? Addictional { get; set; }
        [BackingField("log_date")]
        public DateTime LogDate { get; set; }
    }
}
