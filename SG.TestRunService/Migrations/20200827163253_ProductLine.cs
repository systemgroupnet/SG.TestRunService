using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace SG.TestRunService.Migrations
{
    public partial class ProductLine : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductLine",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(nullable: true),
                    AzureProductBuildDefinitionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductLine", x => x.Id);
                });

            migrationBuilder.Sql("SET IDENTITY_INSERT ProductLine ON;");

            migrationBuilder.Sql(@"
INSERT INTO ProductLine (Id, [Key], AzureProductBuildDefinitionId)
SELECT AzureBuildDefinitionId, LEFT(BuildNumber,LEN(BuildNumber)-CHARINDEX('_', REVERSE(BuildNumber))), AzureBuildDefinitionId
FROM
(SELECT AzureBuildDefinitionId, BuildNumber = MAX(BuildNumber) FROM BuildInfo
GROUP BY AzureBuildDefinitionId) builds");

            migrationBuilder.Sql("SET IDENTITY_INSERT ProductLine OFF;");

            migrationBuilder.RenameColumn(
                name: "AzureProductBuildDefinitionId",
                table: "TestLastState",
                newName: "ProductLineId");

            migrationBuilder.RenameIndex(
                name: "IX_TestLastState_TestCaseId_AzureProductBuildDefinitionId",
                newName: "IX_TestLastState_TestCaseId_ProductLineId",
                table: "TestLastState");

            migrationBuilder.RenameColumn(
                name: "AzureProductBuildDefinitionId",
                table: "TestCaseImpactItem",
                newName: "ProductLineId");

            migrationBuilder.RenameIndex(
                name: "IX_TestCaseImpactItem_AzureProductBuildDefinitionId_TestCaseId",
                newName: "IX_TestCaseImpactItem_ProductLineId_TestCaseId",
                table: "TestCaseImpactItem");

            migrationBuilder.RenameColumn(
                name: "AzureProductBuildDefinitionId",
                table: "TestCaseImpactHistory",
                newName: "ProductLineId");

            migrationBuilder.RenameColumn(
                name: "AzureProductBuildDefinitionId",
                table: "LastImpactUpdate",
                newName: "ProductLineId");

            migrationBuilder.RenameIndex(
                name: "IX_LastImpactUpdate_AzureProductBuildDefinitionId",
                newName: "IX_LastImpactUpdate_ProductLineId",
                table: "LastImpactUpdate");

            migrationBuilder.AddColumn<int>(
                name: "ProductLineId",
                table: "TestRunSession",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql( @"
UPDATE s SET s.ProductLineId=bi.AzureBuildDefinitionId
FROM TestRunSession s
JOIN BuildInfo bi on s.ProductBuildInfoId = bi.Id
WHERE s.ProductLineId = 0");

            migrationBuilder.CreateIndex(
                name: "IX_TestRunSession_ProductLineId",
                table: "TestRunSession",
                column: "ProductLineId");

            migrationBuilder.CreateIndex(
                name: "IX_TestLastState_ProductLineId",
                table: "TestLastState",
                column: "ProductLineId");

            migrationBuilder.AddForeignKey(
                name: "FK_LastImpactUpdate_ProductLine_ProductLineId",
                table: "LastImpactUpdate",
                column: "ProductLineId",
                principalTable: "ProductLine",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestLastState_ProductLine_ProductLineId",
                table: "TestLastState",
                column: "ProductLineId",
                principalTable: "ProductLine",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestCaseImpactItem_ProductLine_ProductLineId",
                table: "TestCaseImpactItem",
                column: "ProductLineId",
                principalTable: "ProductLine",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestRunSession_ProductLine_ProductLineId",
                table: "TestRunSession",
                column: "ProductLineId",
                principalTable: "ProductLine",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            throw new NotSupportedException();
        }
    }
}
