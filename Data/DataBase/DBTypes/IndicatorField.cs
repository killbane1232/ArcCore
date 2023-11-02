using System.ComponentModel.DataAnnotations.Schema;

namespace Arcam.Data.DataBase.DBTypes
{
    [Table("indicator_field")]
    public class IndicatorField
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("indicator_id")]
        [ForeignKey("indicator_id")]
        public virtual Indicator Indicator { get; set; }
        [Column("field_type_id")]
        [ForeignKey("field_type_id")]
        public virtual FieldType FieldType { get; set; }
        [Column("is_input")]
        public int IsInput { get; set; }
    }
}
