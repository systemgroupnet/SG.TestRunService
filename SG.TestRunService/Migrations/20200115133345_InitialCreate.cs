using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SG.TestRunService.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Test",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Azure_TestCaseId = table.Column<int>(nullable: false),
                    Azure_TestBuildId = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Test", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TestCaseImpactCodeSignature",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Signature = table.Column<string>(nullable: false),
                    TestId = table.Column<int>(nullable: false),
                    DateAdded = table.Column<DateTime>(nullable: false),
                    DateRemoved = table.Column<DateTime>(nullable: false),
                    IsDelelted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestCaseImpactCodeSignature", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TestRunSession",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeamProject = table.Column<string>(nullable: true),
                    StartTime = table.Column<DateTime>(nullable: false),
                    FinishTime = table.Column<DateTime>(nullable: true),
                    Azure_ProductBuildId = table.Column<int>(nullable: false),
                    Azure_TestBuildId = table.Column<int>(nullable: false),
                    Azure_ProductBuildNumber = table.Column<string>(nullable: true),
                    Azure_TestBuildNumber = table.Column<string>(nullable: true),
                    SuiteName = table.Column<string>(nullable: true),
                    SourceVersion = table.Column<string>(nullable: true),
                    Outcome = table.Column<int>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestRunSession", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExtraData",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestId = table.Column<int>(nullable: true),
                    TestRunId = table.Column<int>(nullable: true),
                    TestRunSessionId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtraData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExtraData_Test_TestId",
                        column: x => x.TestId,
                        principalTable: "Test",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestLastState",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestId = table.Column<int>(nullable: false),
                    Azure_TestBuildId = table.Column<int>(nullable: false),
                    Azure_ProductBuildId = table.Column<int>(nullable: false),
                    UpdateDate = table.Column<int>(nullable: false),
                    SourceVersion = table.Column<string>(nullable: true),
                    Outcome = table.Column<int>(nullable: false),
                    ShouldBeRun = table.Column<bool>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestLastState", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestLastState_Test_TestId",
                        column: x => x.TestId,
                        principalTable: "Test",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestRun",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestRunSessionId = table.Column<int>(nullable: false),
                    TestId = table.Column<int>(nullable: false),
                    Outcome = table.Column<int>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: true),
                    FinishTime = table.Column<DateTime>(nullable: true),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestRun", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestRun_TestRunSession_TestRunSessionId",
                        column: x => x.TestRunSessionId,
                        principalTable: "TestRunSession",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Attachment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestRunId = table.Column<int>(nullable: true),
                    TestCaseRunId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Data = table.Column<byte[]>(nullable: true),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    TestRunSessionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attachment_TestRun_TestRunId",
                        column: x => x.TestRunId,
                        principalTable: "TestRun",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Attachment_TestRunSession_TestRunSessionId",
                        column: x => x.TestRunSessionId,
                        principalTable: "TestRunSession",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_TestRunId",
                table: "Attachment",
                column: "TestRunId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_TestRunSessionId",
                table: "Attachment",
                column: "TestRunSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExtraData_TestId",
                table: "ExtraData",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_Test_Azure_TestBuildId_Azure_TestCaseId",
                table: "Test",
                columns: new[] { "Azure_TestBuildId", "Azure_TestCaseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestCaseImpactCodeSignature_Signature",
                table: "TestCaseImpactCodeSignature",
                column: "Signature");

            migrationBuilder.CreateIndex(
                name: "IX_TestCaseImpactCodeSignature_TestId_Signature",
                table: "TestCaseImpactCodeSignature",
                columns: new[] { "TestId", "Signature" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestLastState_TestId",
                table: "TestLastState",
                column: "TestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestRun_TestRunSessionId_TestId",
                table: "TestRun",
                columns: new[] { "TestRunSessionId", "TestId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attachment");

            migrationBuilder.DropTable(
                name: "ExtraData");

            migrationBuilder.DropTable(
                name: "TestCaseImpactCodeSignature");

            migrationBuilder.DropTable(
                name: "TestLastState");

            migrationBuilder.DropTable(
                name: "TestRun");

            migrationBuilder.DropTable(
                name: "Test");

            migrationBuilder.DropTable(
                name: "TestRunSession");
        }
    }
}
