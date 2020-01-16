﻿using SG.TestRunService.Common.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SG.TestRunService.Common.Data
{
    public class TestRunSession : IEntity
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
        public IList<TestRun> TestRuns { get; set; }
        public IList<Attachment> Attachments { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
