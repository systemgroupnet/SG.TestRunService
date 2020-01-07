using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.Data
{
    public class TSDbContext : DbContext
    {
        public TSDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestImpactCodeSignature>()
                .HasIndex(s => s.Signature);
            modelBuilder.Entity<TestImpactCodeSignature>()
                .HasIndex(s => new { s.TestId, s.Signature })
                .IsUnique();
        }

        public DbSet<TestRunSession> TestRunSessions { get; set; }
        public DbSet<TestRun> TestRuns { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Attachement> Attachements { get; set; }
        public DbSet<TestImpactCodeSignature> TestCaseImpactCodeSignatures { get; set; }

    }
}
