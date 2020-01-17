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
        public int AzureProductBuildDefinitionId { get; set; }
        public int AzureProductBuildId { get; set; }
        public int AzureTestBuildId { get; set; }
        public string AzureProductBuildNumber { get; set; }
        public string AzureTestBuildNumber { get; set; }
        public string SuiteName { get; set; }
        public string SourceVersion { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? FinishTime { get; set; }
        public TestSessionOutcome Outcome { get; set; }
        public int TestRunCount { get; set; }
        public List<TestRunStatisticItem> RunStatistics { get; set; }

    }
}
