using SG.TestRunService.Common.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SG.TestRunService.Common.Models
{
    public class TestRunSessionResponse
    {
        public int Id { get; set; }
        public string TeamProject { get; set; }
        public int Azure_ProductBuildDefinitionId { get; set; }
        public int Azure_ProductBuildId { get; set; }
        public int Azure_TestBuildId { get; set; }
        public string Azure_ProductBuildNumber { get; set; }
        public string Azure_TestBuildNumber { get; set; }
        public string SuiteName { get; set; }
        public string SourceVersion { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? FinishTime { get; set; }
        public TestSessionOutcome Outcome { get; set; }
        public int TestRunCount { get; set; }
        public List<TestRunStatisticItem> RunStatistics { get; set; }

        public static TestRunSessionResponse FromDataModel(TestRunSession session)
        {
            return new TestRunSessionResponse()
            {
                Id = session.Id,
                TeamProject = session.TeamProject,
                Azure_ProductBuildDefinitionId = session.Azure_ProductBuildDefinitionId,
                Azure_ProductBuildId = session.Azure_ProductBuildId,
                Azure_TestBuildId = session.Azure_TestBuildId,
                Azure_ProductBuildNumber = session.Azure_ProductBuildNumber,
                Azure_TestBuildNumber = session.Azure_TestBuildNumber,
                SuiteName = session.SuiteName,
                SourceVersion = session.SourceVersion,
                StartTime = session.StartTime,
                FinishTime = session.FinishTime,
                Outcome = session.Outcome
            };
        }

        public static IQueryable<TestRunSessionResponse> Project(IQueryable<TestRunSession> sessions)
        {
            return sessions.Select(
                session => new TestRunSessionResponse()
                {
                    Id = session.Id,
                    TeamProject = session.TeamProject,
                    Azure_ProductBuildDefinitionId = session.Azure_ProductBuildDefinitionId,
                    Azure_ProductBuildId = session.Azure_ProductBuildId,
                    Azure_TestBuildId = session.Azure_TestBuildId,
                    Azure_ProductBuildNumber = session.Azure_ProductBuildNumber,
                    Azure_TestBuildNumber = session.Azure_TestBuildNumber,
                    SuiteName = session.SuiteName,
                    SourceVersion = session.SourceVersion,
                    StartTime = session.StartTime,
                    FinishTime = session.FinishTime,
                    Outcome = session.Outcome
                });
        }
    }
}
