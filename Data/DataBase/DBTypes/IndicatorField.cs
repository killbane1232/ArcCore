using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
namespace Arcam.Data.DataBase.DBTypes
{
    [Table("indicator_field")]
    public class IndicatorField
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("indicator_id"), ForeignKey("indicator_id")]
        public long IndicatorId { get; set; }
        public virtual Indicator Indicator { get; set; }
        [Column("field_type_id"), ForeignKey("field_type_id")]
        public long FieldTypeId { get; set; }
        public virtual FieldType FieldType { get; set; }
        [Column("is_input")]
        public bool? IsInput { get; set; }
        [Column("code_name")]
        public string? CodeName { get; set; }
    }
}
