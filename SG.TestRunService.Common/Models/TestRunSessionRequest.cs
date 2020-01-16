using SG.TestRunService.Common.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SG.TestRunService.Common.Models
{
    public class TestRunSessionRequest
    {
        [Required]
        public string TeamProject { get; set; }
        [Required]
        public int Azure_ProductBuildDefinitionId { get; set; }
        [Required]
        public int Azure_ProductBuildId { get; set; }
        [Required]
        public int Azure_TestBuildId { get; set; }
        [Required]
        public string Azure_ProductBuildNumber { get; set; }
        [Required]
        public string Azure_TestBuildNumber { get; set; }
        [Required]
        public string SuiteName { get; set; }
        [Required]
        public string SourceVersion { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        public DateTime? FinishTime { get; set; }
        public TestSessionOutcome Outcome { get; set; }
        public List<TestRunRequest> TestRuns { get; set; }

        public TestRunSession ToDataModel()
        {
            return new TestRunSession()
            {
                TeamProject = TeamProject,
                Azure_ProductBuildDefinitionId = Azure_ProductBuildDefinitionId,
                Azure_ProductBuildId = Azure_ProductBuildId,
                Azure_TestBuildId = Azure_TestBuildId,
                Azure_ProductBuildNumber = Azure_ProductBuildNumber,
                Azure_TestBuildNumber = Azure_TestBuildNumber,
                SourceVersion = SourceVersion,
                StartTime = StartTime,
                FinishTime = FinishTime,
                Outcome = Outcome,
                TestRuns = TestRuns.ConvertAll(thisTestRun => new TestRun()
                {
                    TestCaseId = thisTestRun.TestCaseId,
                    Outcome = thisTestRun.Outcome,
                    StartTime = thisTestRun.StartTime,
                    FinishTime = thisTestRun.FinishTime
                })
            };
        }
    }
}
