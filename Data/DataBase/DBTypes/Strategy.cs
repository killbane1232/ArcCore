using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
namespace Arcam.Data.DataBase.DBTypes
{
    [Table("strategy")]
    public class Strategy
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public string? Name { get; set; }
        [Column("author_id"), ForeignKey("author_id")]
        public virtual long AuthorId { get; set; }
        public virtual User Author { get; set; }
        [Column("moduser_id"), ForeignKey("moduser_id")]
        public long ModUserId { get; set; }
        public virtual User ModUser { get; set; }
        [Column("is_public")]
        public bool IsPublic { get; set; }
        [Column("pair_id"), ForeignKey("pair_id")]
        public long? PairId { get; set; }
        public virtual WorkingPair Pair { get; set; }
        [Column("timing_id"), ForeignKey("timing_id")]
        public long? TimingId { get; set; }
        public virtual Timing? Timing { get; set; }
        [Column("long_avail")]
        public bool IsLong { get; set; }
        [Column("short_avail")]
        public bool IsShort { get; set; }
        [Column("leverage")]
        public int Leverage { get; set; }
        public List<StrategyIndicator> StrategyIndicators { get; set; } = new();
        
        public Strategy CreateCopy(ApplicationContext db)
        {
            var strat = new Strategy();
            strat.Name = null;
            strat.PairId = PairId;
            strat.TimingId = TimingId;
            strat.IsPublic = false;
            strat.AuthorId = AuthorId;
            strat.ModUserId = ModUserId;
            strat.Leverage = Leverage;
            strat.IsLong = IsLong;
            strat.IsShort = IsShort;
            db.Strategy.Update(strat);
            db.SaveChanges();
            foreach (var curindic in StrategyIndicators.OrderBy(x => x.IndicatorId).ThenBy(x => x.IsExit))
            {
                var cpy = curindic.CreateCopy(db, strat.Id);
                db.StrategyIndicator.Update(cpy);
                strat.StrategyIndicators.Add(cpy);
            }
            db.SaveChanges();

            return strat;
        }

        public new string GetHashCode()
        {
            var builder = new StringBuilder();
            builder.Append(Leverage);
            builder.Append(PairId);
            builder.Append(TimingId);
            builder.Append(IsLong);
            builder.Append(IsShort);
            foreach (var indic in StrategyIndicators.OrderBy(x => x.IndicatorId).ThenBy(x => x.IsExit))
            {
                builder.Append(indic.IndicatorId);
                builder.Append(indic.IsExit);
                foreach (var item in indic.InputFields.Values.OrderBy(x => x.IndicatorFieldId))
                {
                    builder.Append(item.IndicatorFieldId);
                    builder.Append(item.IntValue);
                    builder.Append(item.FloatValue ?? 0f);
                }
            }
            return builder.ToString();
        }
    }
}
