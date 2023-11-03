using System.ComponentModel.DataAnnotations.Schema;

namespace Arcam.Data.DataBase.DBTypes
{
    [Table("test_strategy")]
    public class TestStrategy
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("strategy_id"), ForeignKey("strategy_id")]
        public virtual long StrategyId { get; set; }
        public virtual Strategy Strategy { get; set; }
    }
}
