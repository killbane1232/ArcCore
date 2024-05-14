using Arcam.Data.DataTypes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcam.Data.DataBase.DBTypes
{
    [Table("trading_history")]
    public class TradingHistory
    {
        [Column("id")]
        public long Id { get; set; }
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
