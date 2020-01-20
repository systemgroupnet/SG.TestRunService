using System;
using System.Collections.Generic;
using System.Text;

namespace SG.TestRunClientLib
{
    public interface IDevOpsServerHandle
    {
        IReadOnlyList<string> GetChangedFiles(
            string project, int azureProductBuildDefinitionId,
            string fromSourceVersion, string toSourceVersion);
        bool IsChronologicallyAfter(string currentSourceVersion, string baseSourceVersion);
    }

}
