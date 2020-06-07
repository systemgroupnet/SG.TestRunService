using SG.TestRunService.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SG.TestRunClientLib
{
    public interface IDevOpsServerHandle
    {
        IReadOnlyList<string> GetBuildChanges(BuildInfo from, BuildInfo to);
        bool IsChronologicallyAfter(string currentSourceVersion, string baseSourceVersion);
    }
}
