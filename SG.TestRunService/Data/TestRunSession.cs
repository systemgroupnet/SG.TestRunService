using SG.TestRunService.Common.Models;
using SG.TestRunService.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SG.TestRunService.Data
{
    public class TestRunSession : IEntity
    {
        public TestRunSession()
        {
            TestRuns = new List<TestRun>();
            Attachments = new List<Attachment>();
            ExtraData = new List<ExtraData>();
        }

        public int Id { get; set; }
        public int ProductBuildInfoId { get; set; }
        public int AzureTestBuildId { get; set; }
        [Required]
        public string AzureTestBuildNumber { get; set; }
        [Required]
        public string SuiteName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? FinishTime { get; set; }
        public TestSessionOutcome Outcome { get; set; }

        [ForeignKey(nameof(ProductBuildInfoId))]
        public BuildInfo ProductBuildInfo { get; set; }
        public IList<TestRun> TestRuns { get; set; }
        public IList<Attachment> Attachments { get; set; }
        public IList<ExtraData> ExtraData { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
