using System;
using System.Collections.Generic;
using System.Text;

namespace SG.TestRunService.Common.Models
{
    public class TestRequest
    {
        public int Azure_TestCaseId { get; set; }
        public string Title { get; set; }
        public Dictionary<string, ExtraDataValue> ExtraData { get; set; }
    }
}
