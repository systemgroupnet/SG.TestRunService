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

        private Dictionary<int, string[]> _changeSets = new Dictionary<int, string[]>()
        {
            [131] = new[]
            {
                SampleTestRunner.files[0],
                SampleTestRunner.files[1],
                SampleTestRunner.files[2],
            },
            [132] = new[]
            {
                SampleTestRunner.files[1],
                SampleTestRunner.files[2],
            },
            [133] = new[]
            {
                SampleTestRunner.files[2],
            },
            [134] = new[]
            {
                SampleTestRunner.files[4],
            },
            [135] = new[]
            {
                SampleTestRunner.files[3],
                SampleTestRunner.files[0],
            },
        };

        public IReadOnlyList<string> GetChangedFiles(
            string project, int azureProductBuildDefinitionId,
            string fromSourceVersion, string toSourceVersion)
        {
            int start = int.Parse(fromSourceVersion);
            int end = int.Parse(toSourceVersion);

            var result = new List<string>();

            for (int i = start + 1; i <= end; i++)
            {
                if (_changeSets.TryGetValue(i, out var changes))
                    foreach (var c in changes)
                        result.Add(c);
            }
            return result;
        }

        public bool IsChronologicallyAfter(string currentSourceVersion, string baseSourceVersion)
        {
            return true;
        }
    }
}
