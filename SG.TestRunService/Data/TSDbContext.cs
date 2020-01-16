using Microsoft.EntityFrameworkCore;
using SG.TestRunService.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.Data
{
    public class TSDbContext : DbContext
    {
        public TSDbContext(DbContextOptions<TSDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestImpactCodeSignature>()
                .HasIndex(s => s.Signature);
            modelBuilder.Entity<TestImpactCodeSignature>()
                .HasIndex(s => new { s.TestCaseId, s.Signature })
                .IsUnique();
            modelBuilder.Entity<TestCase>()
                .HasIndex(t => new { t.Azure_TestCaseId })
                .IsUnique();
            modelBuilder.Entity<TestLastState>()
                .HasIndex(t => t.TestCaseId)
                .IsUnique();
            modelBuilder.Entity<TestRun>()
                .HasIndex(t => new { t.TestRunSessionId, t.TestCaseId })
                .IsUnique();
        }

        public DbSet<TestCase> TestCase { get; set; }
        public DbSet<TestRunSession> TestRunSession { get; set; }
        public DbSet<TestRun> TestRun { get; set; }
        public DbSet<Attachment> Attachment { get; set; }
        public DbSet<TestImpactCodeSignature> TestCaseImpactCodeSignature { get; set; }
        public DbSet<ExtraData> ExtraData { get; set; }
        public DbSet<TestLastState> TestLastState { get; set; }
    }
}
