using SG.TestRunService.Common.Models;
using SG.TestRunService.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SG.TestRunService.Data
{
    public class TestRunSession : IEntity
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
        public IList<TestRun> TestRuns { get; set; }
        public IList<Attachment> Attachments { get; set; }
        public IList<ExtraData> ExtraData { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
