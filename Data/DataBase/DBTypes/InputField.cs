using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcam.Data.DataBase.DBTypes
{
    [Table("input_field")]
    [PrimaryKey(nameof(IndicatorFieldId), nameof(StrategyIndicatorId))]
    public class InputField
    {
        [Column("field_id"), ForeignKey("field_id")]
        public long IndicatorFieldId { get; set; }
        public IndicatorField IndicatorField { get; set; }
        [Column("strategy_indicator_id"), ForeignKey("strategy_indicator_id")]
        public long StrategyIndicatorId { get; set; }
        public StrategyIndicator StrategyIndicator { get; set; }
        [Column("float_value")]
        public float? FloatValue { get; set; }
        [Column("int_value")]
        public int? IntValue { get; set; }
    }
}
