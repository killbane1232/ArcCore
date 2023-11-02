using System.ComponentModel.DataAnnotations.Schema;

namespace Arcam.Data.DataBase.DBTypes
{
    [Table("test_strategy")]
    public class TestStrategy
    {
        [Column("id")]
        public long Id { get; set; }
        [ForeignKey("strategy_id")]
        public virtual Strategy Strategy { get; set; }
    }
}
