using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcam.Data.DataBase.DBTypes
{
    [Table("account")]
    [PrimaryKey(nameof(UserId), nameof(PlatformId))]
    public class Account
    {
        [Column("user_id"), ForeignKey("user_id")]
        public long UserId { get; set; }
        public User User { get; set; }
        [Column("platform_id"), ForeignKey("platform_id")]
        public long PlatformId { get; set; }
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
