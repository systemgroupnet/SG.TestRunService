﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.Common.Models
{
    public class TestRunResponse
    {
        public int Id { get; set; }
        public int TestCaseId { get; set; }
        public int TestRunSessionId { get; set; }
        public TestRunOutcome Outcome { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? FinishTime { get; set; }
        public Dictionary<string, ExtraDataValue> ExtraData { get; set; }

    }
}
