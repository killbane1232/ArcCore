using System.ComponentModel.DataAnnotations.Schema;

namespace Arcam.Data.DataBase.DBTypes
{
    [Table("token")]
    public class DBToken
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("token")]
        public string Token { get; set; } = "";
        [Column("user_id")]
        public long UserId { get; set; }
    }
}
