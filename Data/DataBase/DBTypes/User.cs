using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
namespace Arcam.Data.DataBase.DBTypes
{
    [Table("user")]
    public class User
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("username")]
        public string Name { get; set; }
        [Column("access"), ForeignKey("access")]
        public long? AccessId { get; set; }
        public virtual AccessType? Access { get; set; }
        [Column("password")]
        public string Password { get; set; }
        [Column("login")]
        public string Login { get; set; }
        [Column("tgid")]
        public long? TelegramId { get; set; } //TODO ???
    }
}
