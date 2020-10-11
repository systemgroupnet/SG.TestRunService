using System;
using System.Collections.Generic;
using System.Text;

namespace SG.TestRunService.Common.Models
{
    public class ProductLineMethodImpactStat
    {
        public string MethodFullName { get; set; }

        public int ImpactedTestCasesCount { get; set; }
    }
}
