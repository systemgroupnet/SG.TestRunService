using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SG.TestRunService.Common.Models
{
    public class TestRunSessionRequest
    {
        [Required]
        public BuildInfo ProductBuild { get; set; }
        [Required]
        public int AzureTestBuildId { get; set; }
        [Required]
        public string AzureTestBuildNumber { get; set; }
        [Required]
        public string SuiteName { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        public DateTime? FinishTime { get; set; }
        public TestSessionOutcome Outcome { get; set; }
        public IList<TestRunRequest> TestRuns { get; set; }
    }
}
