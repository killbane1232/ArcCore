using Arcam.Data.DataBase.DBTypes;
using Microsoft.EntityFrameworkCore;

namespace Arcam.Data.DataBase
{
    public class TestContext : ApplicationContext
    {
        public TestContext() 
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connection = "";
            using (StreamReader reader = new StreamReader($"{Constants.ConfigDirectory}/sql.config"))
            {
                connection = reader.ReadLine() ?? "";
                connection = connection.Replace("Database=arc;", "Database=tstarc;");
            }
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            optionsBuilder
                .UseNpgsql(connection);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Strategy>()
                .HasMany(s => s.StrategyIndicators)
                .WithOne(si => si.Strategy)
                .HasForeignKey(si => si.StrategyId)
                .HasPrincipalKey(s => s.Id);
            modelBuilder.Entity<Indicator>()
                .HasMany(s => s.indicatorFields)
                .WithOne(si => si.Indicator)
                .HasForeignKey(si => si.IndicatorId)
                .HasPrincipalKey(s => s.Id);
            modelBuilder.Entity<AccessType>()
                .HasMany(s => s.MatrixParameters)
                .WithOne(si => si.AccessType)
                .HasForeignKey(si => si.AccessTypeId)
                .HasPrincipalKey(s => s.Id);
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }
    }
}
