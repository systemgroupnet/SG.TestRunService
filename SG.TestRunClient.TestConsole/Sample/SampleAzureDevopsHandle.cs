using SG.TestRunClientLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace SG.TestRunClient.TestConsole.Sample
{
    public class SampleAzureDevopsHandle : IDevOpsServerHandle
    {
        public SampleAzureDevopsHandle()
        {
        }

        public IReadOnlyList<string> GetChangedFiles(
            string project, int azureProductBuildDefinitionId,
            string fromSourceVersion, string toSourceVersion)
        {
            int start = int.Parse(fromSourceVersion);
            int end = int.Parse(toSourceVersion);
            if (131 > start && 131 <= end)
                return new[]
                {
                    SampleTestRunner.files[0],
                    SampleTestRunner.files[1],
                    SampleTestRunner.files[2],
                };
            return Array.Empty<string>();
        }

        public bool IsChronologicallyAfter(string currentSourceVersion, string baseSourceVersion)
        {
            return true;
        }
    }
}
