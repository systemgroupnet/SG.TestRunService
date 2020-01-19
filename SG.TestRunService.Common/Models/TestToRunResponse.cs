using System;
using System.Collections.Generic;
using System.Text;

namespace SG.TestRunService.Common.Models
{
    public class TestToRunResponse
    {
        public int Id { get; set; }
        public int AzureTestCaseId { get; set; }
        public RunReason RunReason { get; set; }
    }
}
