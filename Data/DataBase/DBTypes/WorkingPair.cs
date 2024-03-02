using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
namespace Arcam.Data.DataBase.DBTypes
{
    [Table("working_pair")]
    public class WorkingPair
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("platform_id"), ForeignKey("platform_id")]
        public virtual long PlatformId { get; set; }
        public virtual Platform Platform { get; set; }
    }
}
