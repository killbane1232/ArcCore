using System.ComponentModel.DataAnnotations.Schema;

namespace Arcam.Data.DataBase.DBTypes
{
    [Table("platform")]
    public class Platform
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("url")]
        public string Url { get; set; }
        [Column("classname")]
        public string ClassName { get; set; }
    }
}
