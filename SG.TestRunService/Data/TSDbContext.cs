using Microsoft.EntityFrameworkCore;
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
            ConfigureIndexes(modelBuilder);
            Infrastructure.OnDeleteAttribute.Apply(modelBuilder);
        }

        private static void ConfigureIndexes(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CodeSignature>()
                .HasIndex(s => s.Signature)
                .IsUnique();
            modelBuilder.Entity<TestCaseImpactItem>()
                .HasIndex(s => new { s.TestCaseId, s.AzureProductBuildDefinitionId, s.CodeSignatureId })
                .IsUnique();
            modelBuilder.Entity<TestCase>()
                .HasIndex(t => new { t.AzureTestCaseId })
                .IsUnique();
            modelBuilder.Entity<TestLastState>()
                .HasIndex(t => new { t.TestCaseId, t.AzureProductBuildDefinitionId })
                .IsUnique();
            modelBuilder.Entity<TestRun>()
                .HasIndex(t => new { t.TestRunSessionId, t.TestCaseId })
                .IsUnique();
            modelBuilder.Entity<BuildInfo>()
                .HasIndex(b => new { b.AzureBuildDefinitionId, b.AzureBuildId })
                .IsUnique();
            modelBuilder.Entity<LastImpactUpdate>()
                .HasIndex(e => e.AzureProductBuildDefinitionId)
                .IsUnique();
        }

        public DbSet<TestCase> TestCase { get; }
        public DbSet<BuildInfo> BuildInfo { get; }
        public DbSet<TestRunSession> TestRunSession { get; }
        public DbSet<TestRun> TestRun { get; }
        public DbSet<Attachment> Attachment { get; }
        public DbSet<TestCaseImpactItem> TestCaseImpactItem { get; }
        public DbSet<CodeSignature> CodeSignature { get; }
        public DbSet<ExtraData> ExtraData { get; }
        public DbSet<TestLastState> TestLastState { get; }
        public DbSet<LastImpactUpdate> LastImpactUpdate { get; }
    }
}
