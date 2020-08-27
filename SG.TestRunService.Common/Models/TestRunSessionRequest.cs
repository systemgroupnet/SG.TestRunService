using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SG.TestRunService.Common.Models
{
    public class TestRunSessionRequest : IExtraDataContainer
    {
        [Required]
        public ProductLine ProductLine { get; set; }
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
        public TestRunSessionState State { get; set; }
        public IList<TestRunRequest> TestRuns { get; set; }
        public IDictionary<string, ExtraDataValue> ExtraData { get; set; }
    }
}
