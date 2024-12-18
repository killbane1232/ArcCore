using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
namespace Arcam.Data.DataBase.DBTypes
{
    [Table("account_history")]
    public class AccountHistory
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("account_id")]
        public long AccountId { get; set; }
        [Column("user_id")]
        public long UserId { get; set; }
        [Column("platform_id")]
        public long PlatformId { get; set; }
        [Column("is_long")]
        public bool IsLong { get; set; }
        [Column("enter_date")]
        public DateTime EnterDate { get; set; }
        [Column("exit_date")]
        public DateTime? ExitDate { get; set; }
        [Column("enter_price")]
        public double EnterPrice { get; set; }
        [Column("exit_price")]
        public double? ExitPrice { get; set; }
        [Column("deposit_before")]
        public double DepositBefore { get; set; }
        [Column("deposit_after")]
        public double? DepositAfter { get; set; }
        [Column("leverage")]
        public int Leverage { get; set; }
    }
}
