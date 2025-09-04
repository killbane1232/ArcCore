using Arcam.Data.DataBase.DBTypes;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
namespace Arcam.Data.ClickHouse.DBTypes
{
    [Table("//home/arcam_user"), PrimaryKey("StrategyId")]
    public class TestStrategy
    {
        [Column("strategy_id"), ForeignKey("strategy_id")]
        public virtual long StrategyId { get; set; }
        [Column("date")]
        public DateTime Date { get; set; }
        [Column("strategy_hash")]
        public string? StrategyHash { get; set; }
        [Column("base_strategy_id")]
        public virtual long BaseStrategyId { get; set; }
        [NotMapped]
        public List<TestStrategyTP> TestResultsList { get; set; }
        [NotMapped]
        public Strategy Strategy { get; set; }
    }
}
