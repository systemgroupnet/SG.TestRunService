using SG.TestRunService.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SG.TestRunClientLib
{
    public class TestCaseInfo
    {
        public TestCaseInfo(int id, int azureTestCaseId, string title, RunReason runReason)
        {
            Id = id;
            AzureTestCaseId = azureTestCaseId;
            Title = title;
            RunReason = runReason;
            ExtraData = new Dictionary<string, ExtraDataValue>();
        }

        public int Id { get; }
        public int AzureTestCaseId { get; }
        public string Title { get; }
        public RunReason RunReason { get; }
        public int TestRunId { get; set; }
        public IDictionary<string, ExtraDataValue> ExtraData { get; }
    }
}
