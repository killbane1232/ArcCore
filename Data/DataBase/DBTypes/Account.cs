using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
namespace Arcam.Data.DataBase.DBTypes
{
    [Table("account")]
    [PrimaryKey(nameof(UserId), nameof(PlatformId))]
    public class Account
    {
        [Column("user_id"), ForeignKey("user_id")]
        public long UserId { get; set; }
        public virtual User User { get; set; }
        [Column("platform_id"), ForeignKey("platform_id")]
        public long PlatformId { get; set; }
        public virtual Platform Platform { get; set; }
        [Column("created_at")]
        public DateTime? CreationDate { get; set; }
        [Column("current_strategy"), ForeignKey("current_strategy")]
        public long? StrategyId { get; set; }
        public virtual Strategy? Strategy { get; set; }
        [Column("key")]
        public string Key { get; set; }
        [Column("secret")]
        public string Secret{ get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("is_active")]
        public bool? IsActive { get; set; }
    }
}
