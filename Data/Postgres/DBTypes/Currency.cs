using System.ComponentModel.DataAnnotations.Schema;

namespace Arcam.Data.DataBase.DBTypes
{
    [Table("dic_currency")]
    public class Currency
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("delimeter")]
        public int Delimeter { get; set; }

        internal Currency GetCopy(ApplicationContext db)
        {
            Currency? currency = db.Currency.Where(x => x.Name == Name).ToList().FirstOrDefault(defaultValue: null);
            if (currency == null)
            {
                currency = new Currency 
                { 
                    Delimeter = Delimeter,
                    Name = Name
                };
                db.Currency.Add(currency);
                db.SaveChanges();
            }
            return currency;
        }
    }
}
