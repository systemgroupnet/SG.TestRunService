using System;
using System.Collections.Generic;
using System.Text;

namespace SG.TestRunService.Common.Models
{
    public class PublishImpactChangesResponse
    {
        public IReadOnlyList<ImpactedTestResponse> ImpactedTests { get; set; }
        public IDictionary<string, IReadOnlyList<int>> CodeSignatureImpactedTestCaseIds { get; set; }
        public static PublishImpactChangesResponse Empty()
        {
            return new PublishImpactChangesResponse()
            {
                CodeSignatureImpactedTestCaseIds = new Dictionary<string, IReadOnlyList<int>>(),
                ImpactedTests = new List<ImpactedTestResponse>()
            };
        }
    }
}
