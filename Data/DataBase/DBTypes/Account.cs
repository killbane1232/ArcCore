using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcam.Data.DataBase.DBTypes
{
    [Table("account")]
    [PrimaryKey(nameof(User), nameof(Platform))]
    public class Account
    {
        [Required, Column("user_id")]
        public User User { get; set; }
        [Required, Column("platform_id")]
        public Platform Platform { get; set; }
        [Column("created_at")]
        public DateTime? CreationDate { get; set; }
        [Column("current_strategy"), ForeignKey("current_strategy")]
        public virtual Strategy Strategy { get; set; }
        [Column("key")]
        public string key { get; set; }
        [Column("secret")]
        public string secret{ get; set; }
    }
}
