﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace SG.TestRunService.Common.Models
{
    public class TestRunRequest : IExtraDataContainer
    {
        public TestRunRequest()
        {
            ExtraData = new Dictionary<string, ExtraDataValue>();
        }

        public int TestCaseId { get; set; }
        public TestRunState State { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? FinishTime { get; set; }
        public TestRunOutcome Outcome { get; set; }
        public string ErrorMessage { get; set; }
        public IDictionary<string, ExtraDataValue> ExtraData { get; set; }
    }
}
