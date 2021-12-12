using Benchmark.Entities;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Benchmark.Data
{
    public class CatsDbContext : DbContext
    {
        private readonly bool _enableLazyLoading;

        public CatsDbContext(bool enableLazyLoading = false)
            => _enableLazyLoading = enableLazyLoading;

        public DbSet<Cat> Cats { get; set; }

        public DbSet<Owner> Owners { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                //.LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information)
                //.EnableSensitiveDataLogging()
                .UseLazyLoadingProxies(_enableLazyLoading)
                .UseSqlServer(ConnectionString.DefaultConnection);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder
                .Entity<Cat>()
                .HasOne(c => c.Owner)
                .WithMany(o => o.Cats)
                .HasForeignKey(c => c.OwnerId);
    }
}
