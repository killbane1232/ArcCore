using System.ComponentModel.DataAnnotations.Schema;

namespace Arcam.Data.DataBase.DBTypes
{
    [Table("test_strategy_tp")]
    public class TestStrategyTP
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("test_strategy_id"), ForeignKey("test_strategy_id")]
        public TestStrategy TestStrategyId { get; set; }
        public virtual TestStrategy TestStrategy { get; set; }
        [Column("is_open")]
        public int IsOpen { get; set; }
    }
}
