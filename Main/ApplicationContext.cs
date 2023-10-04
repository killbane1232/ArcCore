using Arcam.Main.Loggers;
using Microsoft.EntityFrameworkCore;

namespace Arcam.Main
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Log> Logs { get; set; }
        public ApplicationContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=usersdb;Username=postgres;Password=здесь_указывается_пароль_от_postgres");
        }
    }
}
