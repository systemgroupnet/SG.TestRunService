﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SG.TestRunService.Data;

namespace SG.TestRunService.Migrations
{
    [DbContext(typeof(TSDbContext))]
    partial class TSDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SG.TestRunService.Data.Attachment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<byte[]>("Data")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("TestCaseRunId")
                        .HasColumnType("int");

                    b.Property<int?>("TestRunId")
                        .HasColumnType("int");

                    b.Property<int?>("TestRunSessionId")
                        .HasColumnType("int");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("TestRunId");

                    b.HasIndex("TestRunSessionId");

                    b.ToTable("Attachment");
                });

            modelBuilder.Entity("SG.TestRunService.Data.ExtraData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("TestId")
                        .HasColumnType("int");

                    b.Property<int?>("TestRunId")
                        .HasColumnType("int");

                    b.Property<int?>("TestRunSessionId")
                        .HasColumnType("int");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<string>("Url")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("TestId");

                    b.ToTable("ExtraData");
                });

            modelBuilder.Entity("SG.TestRunService.Data.Test", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Azure_TestBuildId")
                        .HasColumnType("int");

                    b.Property<int>("Azure_TestCaseId")
                        .HasColumnType("int");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Azure_TestBuildId", "Azure_TestCaseId")
                        .IsUnique();

                    b.ToTable("Test");
                });

            modelBuilder.Entity("SG.TestRunService.Data.TestImpactCodeSignature", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateRemoved")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDelelted")
                        .HasColumnType("bit");

                    b.Property<string>("Signature")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("TestId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Signature");

                    b.HasIndex("TestId", "Signature")
                        .IsUnique();

                    b.ToTable("TestCaseImpactCodeSignature");
                });

            modelBuilder.Entity("SG.TestRunService.Data.TestLastState", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Azure_ProductBuildId")
                        .HasColumnType("int");

                    b.Property<int>("Azure_TestBuildId")
                        .HasColumnType("int");

                    b.Property<int>("Outcome")
                        .HasColumnType("int");

                    b.Property<bool>("ShouldBeRun")
                        .HasColumnType("bit");

                    b.Property<string>("SourceVersion")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TestId")
                        .HasColumnType("int");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<int>("UpdateDate")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TestId")
                        .IsUnique();

                    b.ToTable("TestLastState");
                });

            modelBuilder.Entity("SG.TestRunService.Data.TestRun", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("FinishTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("Outcome")
                        .HasColumnType("int");

                    b.Property<DateTime?>("StartTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("TestId")
                        .HasColumnType("int");

                    b.Property<int>("TestRunSessionId")
                        .HasColumnType("int");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.HasKey("Id");

                    b.HasIndex("TestRunSessionId", "TestId")
                        .IsUnique();

                    b.ToTable("TestRun");
                });

            modelBuilder.Entity("SG.TestRunService.Data.TestRunSession", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Azure_ProductBuildId")
                        .HasColumnType("int");

                    b.Property<string>("Azure_ProductBuildNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Azure_TestBuildId")
                        .HasColumnType("int");

                    b.Property<string>("Azure_TestBuildNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("FinishTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("Outcome")
                        .HasColumnType("int");

                    b.Property<string>("SourceVersion")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("SuiteName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TeamProject")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.HasKey("Id");

                    b.ToTable("TestRunSession");
                });

            modelBuilder.Entity("SG.TestRunService.Data.Attachment", b =>
                {
                    b.HasOne("SG.TestRunService.Data.TestRun", null)
                        .WithMany("Attachments")
                        .HasForeignKey("TestRunId");

                    b.HasOne("SG.TestRunService.Data.TestRunSession", null)
                        .WithMany("Attachments")
                        .HasForeignKey("TestRunSessionId");
                });

            modelBuilder.Entity("SG.TestRunService.Data.ExtraData", b =>
                {
                    b.HasOne("SG.TestRunService.Data.Test", null)
                        .WithMany("ExtraData")
                        .HasForeignKey("TestId");
                });

            modelBuilder.Entity("SG.TestRunService.Data.TestLastState", b =>
                {
                    b.HasOne("SG.TestRunService.Data.Test", "Test")
                        .WithMany()
                        .HasForeignKey("TestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SG.TestRunService.Data.TestRun", b =>
                {
                    b.HasOne("SG.TestRunService.Data.TestRunSession", "Session")
                        .WithMany("TestRuns")
                        .HasForeignKey("TestRunSessionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
