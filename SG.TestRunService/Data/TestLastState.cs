﻿using SG.TestRunService.Common.Models;
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
        public int AzureProductBuildDefinitionId { get; set; }

        public int AzureTestBuildId { get; set; }
        public int AzureProductBuildId { get; set; }
        public int UpdateDate { get; set; }
        public string SourceVersion { get; set; }
        public TestRunOutcome LastOutcome { get; set; }
        public bool ShouldBeRun { get; set; }
        public RunReason? RunReason { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }

        public TestCase TestCase { get; set; }
    }
}
