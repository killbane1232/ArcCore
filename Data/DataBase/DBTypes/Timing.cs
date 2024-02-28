using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
namespace Arcam.Data.DataBase.DBTypes
{
    [Table("dic_timing")]
    public class Timing
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("value")]
        public int Value { get; set; }
        public int GetTimeMultiplier()
        {
            switch (Value)
            {
                case -2:
                    return 1;
                case -1:
                    return 5;
                case 0:
                    return 60;
                case 1:
                    return 24 * 60;
            }
            throw new ArgumentException("WrongTimespanValue");
        }
    }
}
