using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcam.Data.ClickHouse.DBTypes
{
    [Table("//home/arcam_user"), PrimaryKey("PairId", "TimingId", "TimeStamp")]
    public class TradingHistory
    {
        [Column("pair_id")]
        public long PairId { get; set; }
        [Column("timing_id")]
        public long TimingId { get; set; }
        [Column("date")]
        public DateTime TimeStamp { get; set; }
        [Column("open")]
        public double Open { get; set; }
        [Column("close")]
        public double Close { get; set; }
        [Column("high")]
        public double High { get; set; }
        [Column("low")]
        public double Low { get; set; }
        [Column("volume")]
        public double Volume { get; set; }
    }
}
