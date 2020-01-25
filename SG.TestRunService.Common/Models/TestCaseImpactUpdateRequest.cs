using System;
using System.Collections.Generic;
using System.Text;

namespace SG.TestRunService.Common.Models
{
    public class TestCaseImpactUpdateRequest
    {
        public int AzureProductBuildDefinitionId { get; set; }
        public IList<CodeSignature> CodeSignatures { get; set; } 
    }
}
