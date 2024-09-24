using Arcam.Data.ClickHouse.DBTypes;
using ClickHouse.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Arcam.Data.ClickHouse
{
    public class ClickHouseContext : DbContext
    {
        public DbSet<TestStrategy> TestStrategy { get; set; }
        public DbSet<TestStrategyTP> TestStrategyTP { get; set; }
        public DbSet<TradingHistory> TradingHistory { get; set; }
        public ClickHouseContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            var connection = "";
            using (StreamReader reader = new StreamReader($"{Constants.ConfigDirectory}/ch_sql.config"))
            {
                connection = reader.ReadLine() ?? "";
            }

            optionsBuilder
                .UseClickHouse(connection);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestStrategy>()
                .HasMany(s => s.TestResultsList)
                .WithOne(si => si.TestStrategy)
                .HasForeignKey(si => si.TestStrategyId)
                .HasPrincipalKey(s => s.StrategyId);

        }
    }
}
