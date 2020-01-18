using SG.TestRunClient.TestConsole.Sample;
using System;

namespace SG.TestRunClient.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            new SampleTestRunner().RunAsync().Wait();
        }
    }
}
