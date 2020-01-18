using SG.TestRunService.Common.Models;
using SG.TestRunService.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SG.TestRunService.Data
{
    public class TestLastState : IEntity
    {
        public int Id { get; set; }

        public int TestCaseId { get; set; }
        public int AzureProductBuildDefinitionId { get; set; }

        public int ProductBuildInfoId { get; set; }

        public int UpdateDate { get; set; }
        public TestRunOutcome LastOutcome { get; set; }
        public bool ShouldBeRun { get; set; }
        public RunReason? RunReason { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }

        [OnDelete(DeleteBehavior.Cascade)]
        public TestCase TestCase { get; set; }

        [ForeignKey(nameof(ProductBuildInfoId))]
        [OnDelete(DeleteBehavior.Restrict)]
        public BuildInfo ProductBuildInfo { get; set; }
    }
}
