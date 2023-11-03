using Arcam.Data.DataBase.DBTypes;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace Arcam.Data.DataBase
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Account> Account { get; set; }
        public DbSet<ComparedFields> ComparedFields { get; set; }
        public DbSet<CompareType> CompareType { get; set; }
        public DbSet<FieldType> FieldType { get; set; }
        public DbSet<Indicator> Indicator { get; set; }
        public DbSet<IndicatorField> IndicatorField { get; set; }
        public DbSet<InputField> InputField { get; set; }
        public DbSet<Platform> Platform { get; set; }
        public DbSet<Strategy> Strategy { get; set; }
        public DbSet<StrategyIndicator> StrategyIndicator { get; set; }
        public DbSet<TestStrategy> TestStrategy { get; set; }
        public DbSet<TestStrategyTP> TestStrategyTP { get; set; }
        public DbSet<Timing> Timing { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<WorkingPair> WorkingPair { get; set; }
        public ApplicationContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //TODO: загрузка из конфига
            var connection = "";
            using (StreamReader reader = new StreamReader($"{Constants.ConfigDirectory}/sql.config"))
            {
                connection = reader.ReadLine() ?? "";
            }

            optionsBuilder
                .UseNpgsql(connection);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
