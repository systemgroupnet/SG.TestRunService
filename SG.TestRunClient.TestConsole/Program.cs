using SG.TestRunClient.TestConsole.Sample;
using System;

namespace SG.TestRunClient.TestConsole
{
    class Program
    {
        static void Main()
        {
            new SampleTestRunner().RunAsync().Wait();
            Console.ReadKey();
        }
    }
}
