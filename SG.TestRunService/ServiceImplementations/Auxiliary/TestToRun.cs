using SG.TestRunService.Common.Models;
using SG.TestRunService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.ServiceImplementations.Auxiliary
{
    public class TestToRun
    {
        public TestLastState TestLastState { get; set; }
        public int AzureTestCaseId { get; set; }

        private RunReason? _runReason;
        public RunReason RunReason
        {
            get
            {
                if (_runReason.HasValue)
                    return _runReason.Value;
                return TestLastState?.RunReason ?? 0;
            }
            set
            {
                _runReason = value;
            }
        }
    }
}
