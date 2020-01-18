using System;
using System.Collections.Generic;
using System.Text;

namespace SG.TestRunClient.TestConsole.Sample
{
    public class TestCase
    {
        public TestCase(string project, int id, string title, string scriptPath)
        {
            TeamProject = project;
            AzureId = id;
            Title = title;
            ScriptPath = scriptPath;
        }

        public string TeamProject { get; set; }
        public int AzureId { get; set; }
        public string Title { get; set; }
        public string ScriptPath { get; set; }
    }
}
