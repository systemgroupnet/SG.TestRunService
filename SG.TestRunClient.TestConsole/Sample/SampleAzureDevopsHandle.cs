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
            return new List<string> {
                "$/Sample/Project1/Program.cs",
                "$/Sample/Project1/TestClass1.cs"
            };
        }

        public bool IsChronologicallyAfter(string currentSourceVersion, string baseSourceVersion)
        {
            return true;
        }
    }
}
