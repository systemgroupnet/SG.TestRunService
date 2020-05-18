using SG.TestRunService.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.Data.FetchModels
{
    public class FTestRun
    {
        public TestRunResponse Response { get; set; }

        public IList<ExtraData> TestCaseExtraData { get; set; }
    }
}
