using System.ComponentModel.DataAnnotations.Schema;

namespace Arcam.Data.DataBase.DBTypes
{
    [Table("access_type")]
    public class AccessType
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public string Name { get; set; } = "";
        public List<AccessMatrix> MatrixParameters { get; set; } = [];
    }
}
