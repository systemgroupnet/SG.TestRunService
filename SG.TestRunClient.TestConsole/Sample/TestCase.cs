namespace SG.TestRunClient.TestConsole.Sample
{
    public class TestCase
    {
        public TestCase(int id, string title, string scriptPath)
        {
            AzureId = id;
            Title = title;
            ScriptPath = scriptPath;
        }

        public int AzureId { get; set; }
        public string Title { get; set; }
        public string ScriptPath { get; set; }
    }
}
