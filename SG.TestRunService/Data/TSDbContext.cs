﻿using Microsoft.EntityFrameworkCore;
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
                .HasIndex(t => new { t.AzureTestCaseId })
                .IsUnique();
            modelBuilder.Entity<TestLastState>()
                .HasIndex(t => t.TestCaseId)
                .IsUnique();
            modelBuilder.Entity<TestRun>()
                .HasIndex(t => new { t.TestRunSessionId, t.TestCaseId })
                .IsUnique();
        }

        public DbSet<TestCase> TestCase { get; }
        public DbSet<TestRunSession> TestRunSession { get; }
        public DbSet<TestRun> TestRun { get; }
        public DbSet<Attachment> Attachment { get; }
        public DbSet<TestImpactCodeSignature> TestCaseImpactCodeSignature { get; }
        public DbSet<ExtraData> ExtraData { get; }
        public DbSet<TestLastState> TestLastState { get; }
    }
}
