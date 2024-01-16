using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
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
        public bool IsExit { get; set; }
        [NotMapped]
        public Dictionary<string, InputField> InputFields = new();
        [NotMapped]
        public List<ComparedFields> ComparedFields = new();
    }
}
