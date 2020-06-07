using SG.TestRunService.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SG.TestRunClientLib
{
    public interface IDevOpsServerHandle
    {
        Task<IReadOnlyList<string>> GetBuildChangesAsync(BuildInfo from, BuildInfo to);
        bool IsChronologicallyAfter(string currentSourceVersion, string baseSourceVersion);
    }
}
