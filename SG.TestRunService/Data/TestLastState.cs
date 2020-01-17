using SG.TestRunService.Common.Models;
using SG.TestRunService.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SG.TestRunService.Data
{
    public class TestLastState : IEntity
    {
        public int Id { get; set; }
        public int TestCaseId { get; set; }
        public TestCase TestCase { get; set; }
        public int Azure_TestBuildId { get; set; }
        public int Azure_ProductBuildId { get; set; }
        public int UpdateDate { get; set; }
        public string SourceVersion { get; set; }
        public TestRunOutcome LastOutcome { get; set; }
        public bool ShouldBeRun { get; set; }
        public RunReason? RunReason { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
