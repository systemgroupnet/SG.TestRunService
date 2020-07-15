using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SG.TestRunService.Migrations
{
    public partial class ImpactHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TestCaseImpactHistory",
                columns: table => new
                {
                    TestCaseId = table.Column<int>(nullable: false),
                    AzureProductBuildDefinitionId = table.Column<int>(nullable: false),
                    CodeSignatureId = table.Column<int>(nullable: false),
                    ProductBuildInfoId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestCaseImpactHistory", x => new { x.AzureProductBuildDefinitionId, x.CodeSignatureId, x.TestCaseId, x.ProductBuildInfoId });
                    table.ForeignKey(
                        name: "FK_TestCaseImpactHistory_CodeSignature_CodeSignatureId",
                        column: x => x.CodeSignatureId,
                        principalTable: "CodeSignature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestCaseImpactHistory_BuildInfo_ProductBuildInfoId",
                        column: x => x.ProductBuildInfoId,
                        principalTable: "BuildInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestCaseImpactHistory_TestCase_TestCaseId",
                        column: x => x.TestCaseId,
                        principalTable: "TestCase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestCaseImpactHistory_CodeSignatureId",
                table: "TestCaseImpactHistory",
                column: "CodeSignatureId");

            migrationBuilder.CreateIndex(
                name: "IX_TestCaseImpactHistory_ProductBuildInfoId",
                table: "TestCaseImpactHistory",
                column: "ProductBuildInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_TestCaseImpactHistory_TestCaseId",
                table: "TestCaseImpactHistory",
                column: "TestCaseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestCaseImpactHistory");
        }
    }
}
