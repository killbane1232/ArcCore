using Arcam.Data.DataBase.DBTypes;
using Microsoft.EntityFrameworkCore;

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
            optionsBuilder
                //.UseLazyLoadingProxies()
                .UseNpgsql();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().Property(x => x.User).HasConversion(
                User => User.Id,
                x => (User)this.Find(typeof(User), x)
                );
            modelBuilder.Entity<Account>().Property(x => x.Platform).HasConversion(
                User => User.Id,
                x => (Platform)this.Find(typeof(Platform), x)
                );
            modelBuilder.Entity<ComparedFields>().Property(x => x.FieldA).HasConversion(
                User => User.Id,
                x => (IndicatorField)this.Find(typeof(IndicatorField), x)
                );
            modelBuilder.Entity<ComparedFields>().Property(x => x.FieldB).HasConversion(
                User => User.Id,
                x => (IndicatorField)this.Find(typeof(IndicatorField), x)
                );
            modelBuilder.Entity<ComparedFields>().Property(x => x.StrategyIndicator).HasConversion(
                User => User.Id,
                x => (StrategyIndicator)this.Find(typeof(StrategyIndicator), x)
                );
            modelBuilder.Entity<InputField>().Property(x => x.IndicatorField).HasConversion(
                User => User.Id,
                x => (IndicatorField)this.Find(typeof(IndicatorField), x)
                );
            modelBuilder.Entity<InputField>().Property(x => x.StrategyIndicator).HasConversion(
                User => User.Id,
                x => (StrategyIndicator)this.Find(typeof(StrategyIndicator), x)
                );
        }
    }
}
