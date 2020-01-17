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

    }
}
