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
        public long PlatformId { get; set; }
        public virtual Platform Platform { get; set; }
        [Column("main_currency"), ForeignKey("main_currency")]
        public virtual long MainCurrencyId { get; set; }
        public virtual Currency MainCurrency { get; set; }
        [Column("multiplier")]
        public virtual double PointMultiplier { get; set; }
        [Column("point_multiplier")]
        public virtual double Multiplier { get; set; }
        [Column("encounting_type"), ForeignKey("encounting_type")]
        public virtual long EncountingTypeId { get; set; }
        public virtual EncountingType EncountingType { get; set; }
    }
}
