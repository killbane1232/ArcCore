using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcam.Data.DataBase.DBTypes
{
    [Table("compared_fields")]
    [PrimaryKey(nameof(FieldAId), nameof(FieldBId), nameof(StrategyIndicatorId))]
    public class ComparedFields
    {
        [Column("field_a_id"), ForeignKey("field_a_id")]
        public long FieldAId { get; set; }
        public IndicatorField FieldA { get; set; }
        [Column("field_b_id"), ForeignKey("field_b_id")]
        public long FieldBId { get; set; }
        public IndicatorField FieldB { get; set; }
        [Column("strategy_indicator_id"), ForeignKey("strategy_indicator_id")]
        public long StrategyIndicatorId { get; set; }
        public StrategyIndicator StrategyIndicator { get; set; }
        [Column("field_a_to_b_comprassion_id")]
        public long CompareTypeId { get; set; }
        [ForeignKey("field_a_to_b_comprassion_id")]
        public virtual CompareType CompareType { get; set; }
    }
}
