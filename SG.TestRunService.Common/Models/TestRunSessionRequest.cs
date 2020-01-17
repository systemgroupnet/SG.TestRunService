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
        public int AzureProductBuildDefinitionId { get; set; }
        [Required]
        public int AzureProductBuildId { get; set; }
        [Required]
        public int AzureTestBuildId { get; set; }
        [Required]
        public string AzureProductBuildNumber { get; set; }
        [Required]
        public string AzureTestBuildNumber { get; set; }
        [Required]
        public string SuiteName { get; set; }
        [Required]
        public string SourceVersion { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        public DateTime? FinishTime { get; set; }
        public TestSessionOutcome Outcome { get; set; }
        public List<TestRunRequest> TestRuns { get; set; }
    }
}
