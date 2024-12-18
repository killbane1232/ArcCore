﻿using Arcam.Data.DataBase.DBTypes;
using Microsoft.EntityFrameworkCore;

namespace Arcam.Data.DataBase
{
    public abstract class ApplicationContext : DbContext
    {
        public DbSet<Account> Account { get; set; }
        public DbSet<AccountHistory> AccountHistory { get; set; }
        public DbSet<ComparedFields> ComparedFields { get; set; }
        public DbSet<CompareType> CompareType { get; set; }
        public DbSet<FieldType> FieldType { get; set; }
        public DbSet<Indicator> Indicator { get; set; }
        public DbSet<IndicatorField> IndicatorField { get; set; }
        public DbSet<InputField> InputField { get; set; }
        public DbSet<Platform> Platform { get; set; }
        public DbSet<Strategy> Strategy { get; set; }
        public DbSet<StrategyIndicator> StrategyIndicator { get; set; }
        public DbSet<Timing> Timing { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<WorkingPair> WorkingPair { get; set; }
        public DbSet<DBToken> Token { get; set; }
        public DbSet<AccessParameter> AccessParameter { get; set; }
        public DbSet<AccessType> AccessType { get; set; }
        public DbSet<AccessMatrix> AccessMatrix { get; set; }
        public DbSet<Currency> Currency { get; set; }
        public DbSet<EncountingType> EncountingType { get; set; }
        public ApplicationContext()
        {
            Database.EnsureCreated();
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
