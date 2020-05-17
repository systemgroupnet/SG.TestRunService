using SG.TestRunService.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.Data.FetchModels
{
    internal class FTestRunSession
    {
        public TestRunSessionResponse Response { get; set; }
        public IList<ExtraData> ExtraData { get; set; }
    }
}
