using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
namespace Arcam.Data.DataBase.DBTypes
{
    [Table("compared_fields")]
    [PrimaryKey(nameof(FieldAId), nameof(FieldBId), nameof(StrategyIndicatorId))]
    public class ComparedFields
    {
        [Column("field_a_id"), ForeignKey("field_a_id")]
        public long FieldAId { get; set; }
        public virtual IndicatorField FieldA { get; set; }
        [Column("field_b_id"), ForeignKey("field_b_id")]
        public long FieldBId { get; set; }
        public virtual IndicatorField FieldB { get; set; }
        [Column("strategy_indicator_id"), ForeignKey("strategy_indicator_id")]
        public long StrategyIndicatorId { get; set; }
        public virtual StrategyIndicator StrategyIndicator { get; set; }
        [Column("field_a_to_b_comprassion_id"), ForeignKey("field_a_to_b_comprassion_id")]
        public long CompareTypeId { get; set; }
        public virtual CompareType CompareType { get; set; }
    }
}
