using System.ComponentModel.DataAnnotations.Schema;

namespace Arcam.Data.DataBase.DBTypes
{
    [Table("strategy")]
    public class Strategy
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("author_id"), ForeignKey("author_id")]
        public virtual long AuthorId { get; set; }
        public virtual User Author { get; set; }
        [Column("moduser_id"), ForeignKey("moduser_id")]
        public long ModUserId { get; set; }
        public virtual User ModUser { get; set; }
        [Column("is_public")]
        public int IsPublic { get; set; }
        [Column("pair_id"), ForeignKey("pair_id")]
        public  long PairId { get; set; }
        public virtual WorkingPair Pair { get; set; }
        [Column("timing_id"), ForeignKey("timing_id")]
        public  long TimingId { get; set; }
        public virtual Timing Timing { get; set; }
        [Column("long_avail")]
        public int IsLong { get; set; }
        [Column("short_avail")]
        public int IsShort { get; set; }
        [Column("leverage")]
        public int Leverage { get; set; }
        [NotMapped]
        public List<StrategyIndicator> StrategyIndicators { get; set; }
    }
}
