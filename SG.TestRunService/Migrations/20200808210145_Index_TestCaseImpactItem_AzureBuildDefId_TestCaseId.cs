using Microsoft.EntityFrameworkCore.Migrations;

namespace SG.TestRunService.Migrations
{
    public partial class Index_TestCaseImpactItem_AzureBuildDefId_TestCaseId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TestCaseImpactItem_AzureProductBuildDefinitionId_TestCaseId",
                table: "TestCaseImpactItem",
                columns: new[] { "AzureProductBuildDefinitionId", "TestCaseId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TestCaseImpactItem_AzureProductBuildDefinitionId_TestCaseId",
                table: "TestCaseImpactItem");
        }
    }
}
