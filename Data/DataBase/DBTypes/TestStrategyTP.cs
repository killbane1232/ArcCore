using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
namespace Arcam.Data.DataBase.DBTypes
{
    [Table("test_strategy_tp")]
    public class TestStrategyTP
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("test_strategy_id"), ForeignKey("test_strategy_id")]
        public long TestStrategyId { get; set; }
        public virtual TestStrategy TestStrategy { get; set; }
        [Column("is_open")]
        public bool IsOpen { get; set; }
        [Column("date")]
        public DateTime Date { get; set; }
    }
}
