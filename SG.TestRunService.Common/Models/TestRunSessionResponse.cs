using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SG.TestRunService.Common.Models
{
    public class TestRunSessionResponse
    {
        public int Id { get; set; }
        public BuildInfo ProductBuild { get; set; }
        public int AzureTestBuildId { get; set; }
        public string AzureTestBuildNumber { get; set; }
        public string SuiteName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? FinishTime { get; set; }
        public TestRunSessionState State { get; set; }
        public int TestRunCount { get; set; }
        public IList<TestRunStatisticItem> RunStatistics { get; set; }
    }
}
