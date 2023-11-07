using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
namespace Arcam.Data.DataBase.DBTypes
{
    [Table("dic_comprassion")]
    public class CompareType
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("field_type_id"), ForeignKey("field_type_id")]
        public long FieldTypeId { get; set; }
        public virtual FieldType FieldType { get; set; }
        [Column("inverse_comprassion")]
        public long? InverseId { get; set; }
    }
}
