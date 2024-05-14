using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
namespace Arcam.Data.DataBase.DBTypes
{
    [Table("test_strategy")]
    public class TestStrategy
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("strategy_id"), ForeignKey("strategy_id")]
        public virtual long StrategyId { get; set; }
        [NotMapped]
        public virtual Strategy Strategy { get; set; }
        [Column("date")]
        public DateTime Date { get; set; }
        [Column("strategy_hash")]
        public string? StrategyHash { get; set; }
        [Column("base_strategy_id")]
        public virtual long BaseStrategyId { get; set; }
        [NotMapped]
        public List<TestStrategyTP> TestResultsList { get; set; }
    }
}
