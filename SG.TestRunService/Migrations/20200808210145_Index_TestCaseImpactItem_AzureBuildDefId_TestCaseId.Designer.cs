﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SG.TestRunService.Data;

namespace SG.TestRunService.Migrations
{
    [DbContext(typeof(TSDbContext))]
    [Migration("20200808210145_Index_TestCaseImpactItem_AzureBuildDefId_TestCaseId")]
    partial class Index_TestCaseImpactItem_AzureBuildDefId_TestCaseId
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.6")
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

            modelBuilder.Entity("SG.TestRunService.Data.BuildInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AzureBuildDefinitionId")
                        .HasColumnType("int");

                    b.Property<int>("AzureBuildId")
                        .HasColumnType("int");

                    b.Property<string>("BuildNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("SourceVersion")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TeamProject")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AzureBuildDefinitionId", "AzureBuildId")
                        .IsUnique();

                    b.ToTable("BuildInfo");
                });

            modelBuilder.Entity("SG.TestRunService.Data.CodeSignature", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Path")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Signature")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.Property<byte>("Type")
                        .HasColumnType("tinyint");

                    b.HasKey("Id");

                    b.HasIndex("Signature")
                        .IsUnique();

                    b.ToTable("CodeSignature");
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

                    b.Property<int?>("TestCaseId")
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

                    b.HasIndex("TestCaseId");

                    b.HasIndex("TestRunId");

                    b.HasIndex("TestRunSessionId");

                    b.ToTable("ExtraData");
                });

            modelBuilder.Entity("SG.TestRunService.Data.LastImpactUpdate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AzureProductBuildDefinitionId")
                        .HasColumnType("int");

                    b.Property<int>("ProductBuildInfoId")
                        .HasColumnType("int");

                    b.Property<int?>("TestRunSessionId")
                        .HasColumnType("int");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("AzureProductBuildDefinitionId")
                        .IsUnique();

                    b.HasIndex("ProductBuildInfoId");

                    b.HasIndex("TestRunSessionId");

                    b.ToTable("LastImpactUpdate");
                });

            modelBuilder.Entity("SG.TestRunService.Data.TestCase", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AzureTestCaseId")
                        .HasColumnType("int");

                    b.Property<string>("TeamProject")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AzureTestCaseId")
                        .IsUnique();

                    b.ToTable("TestCase");
                });

            modelBuilder.Entity("SG.TestRunService.Data.TestCaseImpactHistory", b =>
                {
                    b.Property<int>("AzureProductBuildDefinitionId")
                        .HasColumnType("int");

                    b.Property<int>("CodeSignatureId")
                        .HasColumnType("int");

                    b.Property<int>("TestCaseId")
                        .HasColumnType("int");

                    b.Property<int>("ProductBuildInfoId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.HasKey("AzureProductBuildDefinitionId", "CodeSignatureId", "TestCaseId", "ProductBuildInfoId");

                    b.HasIndex("CodeSignatureId");

                    b.HasIndex("ProductBuildInfoId");

                    b.HasIndex("TestCaseId");

                    b.ToTable("TestCaseImpactHistory");
                });

            modelBuilder.Entity("SG.TestRunService.Data.TestCaseImpactItem", b =>
                {
                    b.Property<int>("AzureProductBuildDefinitionId")
                        .HasColumnType("int");

                    b.Property<int>("CodeSignatureId")
                        .HasColumnType("int");

                    b.Property<int>("TestCaseId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateRemoved")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.HasKey("AzureProductBuildDefinitionId", "CodeSignatureId", "TestCaseId");

                    b.HasIndex("CodeSignatureId");

                    b.HasIndex("TestCaseId");

                    b.HasIndex("AzureProductBuildDefinitionId", "TestCaseId");

                    b.ToTable("TestCaseImpactItem");
                });

            modelBuilder.Entity("SG.TestRunService.Data.TestLastState", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AzureProductBuildDefinitionId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("LastImpactedDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastImpactedProductBuildInfoId")
                        .HasColumnType("int");

                    b.Property<int>("LastOutcome")
                        .HasColumnType("int");

                    b.Property<DateTime>("LastOutcomeDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("LastOutcomeProductBuildInfoId")
                        .HasColumnType("int");

                    b.Property<int?>("RunReason")
                        .HasColumnType("int");

                    b.Property<bool>("ShouldBeRun")
                        .HasColumnType("bit");

                    b.Property<int>("TestCaseId")
                        .HasColumnType("int");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.HasKey("Id");

                    b.HasIndex("LastImpactedProductBuildInfoId");

                    b.HasIndex("LastOutcomeProductBuildInfoId");

                    b.HasIndex("TestCaseId", "AzureProductBuildDefinitionId")
                        .IsUnique();

                    b.ToTable("TestLastState");
                });

            modelBuilder.Entity("SG.TestRunService.Data.TestRun", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ErrorMessage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("FinishTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("Outcome")
                        .HasColumnType("int");

                    b.Property<DateTime?>("StartTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<int>("TestCaseId")
                        .HasColumnType("int");

                    b.Property<int>("TestRunSessionId")
                        .HasColumnType("int");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.HasKey("Id");

                    b.HasIndex("TestCaseId");

                    b.HasIndex("TestRunSessionId", "TestCaseId")
                        .IsUnique();

                    b.ToTable("TestRun");
                });

            modelBuilder.Entity("SG.TestRunService.Data.TestRunSession", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AzureTestBuildId")
                        .HasColumnType("int");

                    b.Property<string>("AzureTestBuildNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("FinishTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("ProductBuildInfoId")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<string>("SuiteName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.HasKey("Id");

                    b.HasIndex("ProductBuildInfoId");

                    b.ToTable("TestRunSession");
                });

            modelBuilder.Entity("SG.TestRunService.Data.Attachment", b =>
                {
                    b.HasOne("SG.TestRunService.Data.TestRun", null)
                        .WithMany("Attachments")
                        .HasForeignKey("TestRunId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SG.TestRunService.Data.TestRunSession", null)
                        .WithMany("Attachments")
                        .HasForeignKey("TestRunSessionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SG.TestRunService.Data.ExtraData", b =>
                {
                    b.HasOne("SG.TestRunService.Data.TestCase", null)
                        .WithMany("ExtraData")
                        .HasForeignKey("TestCaseId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SG.TestRunService.Data.TestRun", null)
                        .WithMany("ExtraData")
                        .HasForeignKey("TestRunId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SG.TestRunService.Data.TestRunSession", null)
                        .WithMany("ExtraData")
                        .HasForeignKey("TestRunSessionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SG.TestRunService.Data.LastImpactUpdate", b =>
                {
                    b.HasOne("SG.TestRunService.Data.BuildInfo", "ProductBuildInfo")
                        .WithMany()
                        .HasForeignKey("ProductBuildInfoId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SG.TestRunService.Data.TestRunSession", "TestRunSession")
                        .WithMany()
                        .HasForeignKey("TestRunSessionId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("SG.TestRunService.Data.TestCaseImpactHistory", b =>
                {
                    b.HasOne("SG.TestRunService.Data.CodeSignature", "CodeSignature")
                        .WithMany()
                        .HasForeignKey("CodeSignatureId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SG.TestRunService.Data.BuildInfo", "ProductBuildInfo")
                        .WithMany()
                        .HasForeignKey("ProductBuildInfoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SG.TestRunService.Data.TestCase", "TestCase")
                        .WithMany()
                        .HasForeignKey("TestCaseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SG.TestRunService.Data.TestCaseImpactItem", b =>
                {
                    b.HasOne("SG.TestRunService.Data.CodeSignature", "CodeSignature")
                        .WithMany()
                        .HasForeignKey("CodeSignatureId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SG.TestRunService.Data.TestCase", "TestCase")
                        .WithMany()
                        .HasForeignKey("TestCaseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SG.TestRunService.Data.TestLastState", b =>
                {
                    b.HasOne("SG.TestRunService.Data.BuildInfo", "LastImpactedProductBuildInfo")
                        .WithMany()
                        .HasForeignKey("LastImpactedProductBuildInfoId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("SG.TestRunService.Data.BuildInfo", "LastOutcomeProductBuildInfo")
                        .WithMany()
                        .HasForeignKey("LastOutcomeProductBuildInfoId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SG.TestRunService.Data.TestCase", "TestCase")
                        .WithMany()
                        .HasForeignKey("TestCaseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SG.TestRunService.Data.TestRun", b =>
                {
                    b.HasOne("SG.TestRunService.Data.TestCase", "TestCase")
                        .WithMany()
                        .HasForeignKey("TestCaseId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SG.TestRunService.Data.TestRunSession", "TestRunSession")
                        .WithMany("TestRuns")
                        .HasForeignKey("TestRunSessionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("SG.TestRunService.Data.TestRunSession", b =>
                {
                    b.HasOne("SG.TestRunService.Data.BuildInfo", "ProductBuildInfo")
                        .WithMany()
                        .HasForeignKey("ProductBuildInfoId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
