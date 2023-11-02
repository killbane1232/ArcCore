using System.ComponentModel.DataAnnotations.Schema;

namespace Arcam.Data.DataBase.DBTypes
{
    [Table("user")]
    public class User
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("username")]
        public string? Name { get; set; }
        [Column("access")]
        public long? Access { get; set; }
        [Column("password")]
        public string? Password { get; set; }
        [Column("login")]
        public string? Login { get; set; }
    }
}
