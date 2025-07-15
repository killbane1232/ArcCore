using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
namespace Arcam.Data.DataBase.DBTypes
{
    [Table("input_field")]
    [PrimaryKey(nameof(IndicatorFieldId), nameof(StrategyIndicatorId))]
    public class InputField
    {
        [Column("field_id"), ForeignKey("field_id")]
        public long IndicatorFieldId { get; set; }
        public virtual IndicatorField IndicatorField { get; set; }
        [Column("strategy_indicator_id"), ForeignKey("strategy_indicator_id")]
        public long StrategyIndicatorId { get; set; }
        public virtual StrategyIndicator StrategyIndicator { get; set; }
        [Column("float_value")]
        public float? FloatValue { get; set; }
        [Column("int_value")]
        public int? IntValue { get; set; }
        //public string? StringValue { get; set; }

        public InputField CreateCopy(ApplicationContext db)
        {
            var field = new InputField();
            var iField = db.IndicatorField.Where(x => x.CodeName == IndicatorField.CodeName).First();
            field.IndicatorFieldId = iField.Id;
            field.IntValue = IntValue;
            field.FloatValue = FloatValue;
            return field;
        }
    }
}
