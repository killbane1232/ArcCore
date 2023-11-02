using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcam.Data.DataBase.DBTypes
{
    [Table("compared_fields")]
    [PrimaryKey(nameof(FieldA), nameof(FieldB), nameof(StrategyIndicator))]
    public class ComparedFields
    {
        [Column("field_a_id")]
        [ForeignKey("field_a_id")]
        public IndicatorField FieldA { get; set; }
        [Column("field_b_id")]
        [ForeignKey("field_b_id")]
        public IndicatorField FieldB { get; set; }
        [Column("strategy_indicator_id"), ForeignKey("strategy_indicator_id")]
        public StrategyIndicator StrategyIndicator { get; set; }
        [Column("field_a_to_b_comprassion_id")]
        [ForeignKey("field_a_to_b_comprassion_id")]
        public virtual CompareType CompareType { get; set; }
    }
}
