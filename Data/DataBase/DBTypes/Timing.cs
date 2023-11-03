using System.ComponentModel.DataAnnotations.Schema;

namespace Arcam.Data.DataBase.DBTypes
{
    [Table("dic_timing")]
    public class Timing
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("value")]
        public int Value { get; set; }
    }
}
