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
    }
}
