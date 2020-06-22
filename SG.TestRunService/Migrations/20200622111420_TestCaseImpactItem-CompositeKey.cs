using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SG.TestRunService.Migrations
{
    public partial class TestCaseImpactItemCompositeKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First drop all indexes, to prevent rebuilding them multiple times
            migrationBuilder.DropIndex(
                name: "IX_TestCaseImpactItem_AzureProductBuildDefinitionId_IsDeleted",
                table: "TestCaseImpactItem");

            migrationBuilder.DropIndex(
                name: "IX_TestCaseImpactItem_TestCaseId_AzureProductBuildDefinitionId_CodeSignatureId",
                table: "TestCaseImpactItem");

            migrationBuilder.DropIndex(
                name: "IX_TestCaseImpactItem_CodeSignatureId",
                table: "TestCaseImpactItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestCaseImpactItem",
                table: "TestCaseImpactItem");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "TestCaseImpactItem");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestCaseImpactItem",
                table: "TestCaseImpactItem",
                columns: new[] { "AzureProductBuildDefinitionId", "CodeSignatureId", "TestCaseId" });

            // Recreate index
            migrationBuilder.CreateIndex(
                name: "IX_TestCaseImpactItem_CodeSignatureId",
                table: "TestCaseImpactItem",
                column: "CodeSignatureId");

            migrationBuilder.CreateIndex(
                name: "IX_TestCaseImpactItem_TestCaseId",
                table: "TestCaseImpactItem",
                column: "TestCaseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TestCaseImpactItem_TestCaseId",
                table: "TestCaseImpactItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestCaseImpactItem",
                table: "TestCaseImpactItem");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "TestCaseImpactItem",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestCaseImpactItem",
                table: "TestCaseImpactItem",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TestCaseImpactItem_AzureProductBuildDefinitionId_IsDeleted",
                table: "TestCaseImpactItem",
                columns: new[] { "AzureProductBuildDefinitionId", "IsDeleted" })
                .Annotation("SqlServer:Include", new[] { "TestCaseId", "CodeSignatureId" });

            migrationBuilder.CreateIndex(
                name: "IX_TestCaseImpactItem_TestCaseId_AzureProductBuildDefinitionId_CodeSignatureId",
                table: "TestCaseImpactItem",
                columns: new[] { "TestCaseId", "AzureProductBuildDefinitionId", "CodeSignatureId" },
                unique: true);
        }
    }
}
