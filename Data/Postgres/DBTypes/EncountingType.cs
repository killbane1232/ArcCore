using System.ComponentModel.DataAnnotations.Schema;

namespace Arcam.Data.DataBase.DBTypes
{
    [Table("dic_encounting_type")]
    public class EncountingType
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public string Name { get; set; }

        internal EncountingType GetCopy(ApplicationContext db)
        {
            EncountingType? currency = db.EncountingType.Where(x => x.Name == Name).ToList().FirstOrDefault(defaultValue: null);
            if (currency == null)
            {
                currency = new EncountingType
                {
                    Name = Name
                };
                db.EncountingType.Add(currency);
                db.SaveChanges();
            }
            return currency;
        }
    }
}
