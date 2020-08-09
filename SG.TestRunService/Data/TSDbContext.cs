using Microsoft.EntityFrameworkCore;
using System;

namespace SG.TestRunService.Data
{
    public class TSDbContext : DbContext
    {
        public TSDbContext(DbContextOptions<TSDbContext> options)
            : base(options)
        {
            Database.SetCommandTimeout(TimeSpan.FromMinutes(3));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestCaseImpactItem>()
                .HasKey(i => new { i.AzureProductBuildDefinitionId, i.CodeSignatureId, i.TestCaseId });

            modelBuilder.Entity<TestCaseImpactHistory>()
                .HasKey(i => new { i.AzureProductBuildDefinitionId, i.CodeSignatureId, i.TestCaseId, i.ProductBuildInfoId });

            ConfigureIndexes(modelBuilder);
            Infrastructure.OnDeleteAttribute.Apply(modelBuilder);
        }


        private static void ConfigureIndexes(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CodeSignature>()
                .HasIndex(s => s.Signature)
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
            modelBuilder.Entity<TestCaseImpactItem>()
                .HasIndex(e => new { e.AzureProductBuildDefinitionId, e.TestCaseId });
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
        public DbSet<TestCaseImpactHistory> TestCaseImpactHistory { get; set; }
    }
}
