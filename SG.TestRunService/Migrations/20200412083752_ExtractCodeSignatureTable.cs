using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SG.TestRunService.Migrations
{
    public partial class ExtractCodeSignatureTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TestCaseImpactCodeSignature_TestCaseId_AzureProductBuildDefinitionId_Signature",
                table: "TestCaseImpactCodeSignature");

            migrationBuilder.CreateTable(
                name: "CodeSignature",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Signature = table.Column<string>(maxLength: 50, nullable: false),
                    Path = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodeSignature", x => x.Id);
                });

            // Copy signatures to new table
            migrationBuilder.Sql(@"
INSERT INTO CodeSignature (Signature)
SELECT DISTINCT Signature
FROM TestCaseImpactCodeSignature");

            // Copy file paths
            migrationBuilder.Sql(@"
UPDATE cs
SET
	[Path] = (SELECT TOP 1 FilePath
	          FROM TestCaseImpactCodeSignature tci
			  WHERE tci.Signature=cs.Signature)
FROM CodeSignature cs");

            // Create index to help find signatures faster
            migrationBuilder.CreateIndex(
                name: "IX_CodeSignature_Signature",
                table: "CodeSignature",
                column: "Signature",
                unique: true);

            migrationBuilder.CreateTable(
                name: "TestCaseImpactItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestCaseId = table.Column<int>(nullable: false),
                    AzureProductBuildDefinitionId = table.Column<int>(nullable: false),
                    CodeSignatureId = table.Column<int>(nullable: false),
                    DateAdded = table.Column<DateTime>(nullable: false),
                    DateRemoved = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestCaseImpactItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestCaseImpactItem_CodeSignature_CodeSignatureId",
                        column: x => x.CodeSignatureId,
                        principalTable: "CodeSignature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestCaseImpactItem_TestCase_TestCaseId",
                        column: x => x.TestCaseId,
                        principalTable: "TestCase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Insert impact data into new table
            migrationBuilder.Sql(@"
INSERT INTO TestCaseImpactItem (TestCaseId, AzureProductBuildDefinitionId, DateAdded, DateRemoved, IsDeleted, CodeSignatureId)
SELECT
	tci.TestCaseId, tci.AzureProductBuildDefinitionId, tci.DateAdded, tci.DateRemoved, tci.IsDeleted,
	(SELECT Id FROM CodeSignature cs WHERE cs.Signature = tci.Signature)
FROM TestCaseImpactCodeSignature tci");


            migrationBuilder.DropTable(
                name: "TestCaseImpactCodeSignature");

            migrationBuilder.CreateIndex(
                name: "IX_TestCaseImpactItem_CodeSignatureId",
                table: "TestCaseImpactItem",
                column: "CodeSignatureId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestCaseImpactItem");

            migrationBuilder.DropTable(
                name: "CodeSignature");

            migrationBuilder.CreateTable(
                name: "TestCaseImpactCodeSignature",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AzureProductBuildDefinitionId = table.Column<int>(type: "int", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateRemoved = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Signature = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TestCaseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestCaseImpactCodeSignature", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestCaseImpactCodeSignature_TestCase_TestCaseId",
                        column: x => x.TestCaseId,
                        principalTable: "TestCase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestCaseImpactCodeSignature_Signature",
                table: "TestCaseImpactCodeSignature",
                column: "Signature");

            migrationBuilder.CreateIndex(
                name: "IX_TestCaseImpactCodeSignature_TestCaseId_AzureProductBuildDefinitionId_Signature",
                table: "TestCaseImpactCodeSignature",
                columns: new[] { "TestCaseId", "AzureProductBuildDefinitionId", "Signature" },
                unique: true);
        }
    }
}
