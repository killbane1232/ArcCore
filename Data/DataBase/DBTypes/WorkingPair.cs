using System.ComponentModel.DataAnnotations.Schema;

namespace Arcam.Data.DataBase.DBTypes
{
    [Table("working_pair")]
    public class WorkingPair
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [ForeignKey("platform_id")]
        public virtual Platform platform { get; set; }
    }
}
