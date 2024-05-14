using System.ComponentModel.DataAnnotations.Schema;

namespace Arcam.Data.DataBase.DBTypes
{
    [Table("dic_access_parameter")]
    public class AccessParameter
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public string Name { get; set; } = "";
        [Column("access_name")]
        public string Description { get; set; } = "";
    }
}
