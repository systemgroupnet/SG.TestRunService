using System;
using System.Collections.Generic;
using System.Text;

namespace SG.TestRunService.Common.Models
{
    public class ImpactedTestResponse
    {
        public int TestCaseId { get; set; }
        public int AzureTestCaseId { get; set; }

        public override bool Equals(object obj)
        {
            return obj is ImpactedTestResponse response &&
                   TestCaseId == response.TestCaseId &&
                   AzureTestCaseId == response.AzureTestCaseId;
        }

        public override int GetHashCode()
        {
            int hashCode = 1447981385;
            hashCode = hashCode * -1521134295 + TestCaseId.GetHashCode();
            hashCode = hashCode * -1521134295 + AzureTestCaseId.GetHashCode();
            return hashCode;
        }
    }
}
