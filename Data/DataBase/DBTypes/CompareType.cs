using System.ComponentModel.DataAnnotations.Schema;

namespace Arcam.Data.DataBase.DBTypes
{
    [Table("dic_comprassion")]
    public class CompareType
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("field_type_id")]
        public virtual long FieldTypeId { get; set; }
        [ForeignKey("field_type_id")]
        public virtual FieldType FieldType { get; set; }
        [Column("inverse_comprassion")]
        public long? InverseId { get; set; }
    }
}
