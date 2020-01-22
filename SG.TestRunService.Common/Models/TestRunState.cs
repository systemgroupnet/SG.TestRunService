using System;
using System.Collections.Generic;
using System.Text;

namespace SG.TestRunService.Common.Models
{
    public enum TestRunState
    {
        NotStarted = 0,
        FixtureQueued = 1,
        WaitingForWeb = 2,
        Running = 3,
        Finished = 10
    }
}
