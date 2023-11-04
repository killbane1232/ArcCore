using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcam.Data.DataBase.DBTypes
{
    [Table("strategy_indicator")]
    public class StrategyIndicator
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("strategy_id"), ForeignKey("strategy_id")]
        public long StrategyId { get; set; }
        public virtual Strategy Strategy { get; set; }
        [Column("indicator_id"), ForeignKey("indicator_id")]
        public long IndicatorId { get; set; }
        public virtual Indicator Indicator { get; set; }
        [Column("is_exit")]
        public int? IsExit { get; set; }
        [NotMapped]
        public Dictionary<string, InputField> InputFields = new Dictionary<string, InputField>();
        [NotMapped]
        public List<ComparedFields> ComparedFields = new List<ComparedFields>();
    }
}
