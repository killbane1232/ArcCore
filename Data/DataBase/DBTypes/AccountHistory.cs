using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
namespace Arcam.Data.DataBase.DBTypes
{
    [Table("account_history")]
    public class Account
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("is_long")]
        public bool IsLong { get; set; }
        [Column("enter_date")]
        public DateTime EnterDate { get; set; }
        [Column("exit_date")]
        public DateTime ExitDate { get; set; }
        [Column("enter_price")]
        public double EnterPrice { get; set; }
        [Column("exit_price")]
        public double ExitPrice { get; set; }
        [Column("leverage")]
        public int Leverage { get; set; }
    }
}
