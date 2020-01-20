using System;
using System.Collections.Generic;
using System.Text;

namespace SG.TestRunClientLib
{
    public interface ITestRunClientConfiguration
    {
        string TestRunServiceUrl { get; }
        TestOlderVersionBehavior RunForOlderVersionBeahvior { get; }
    }

    public enum TestOlderVersionBehavior
    {
        Fail,
        RunNotSuccessfulTests,
        RunImpactedAndNotSuccessfulTests,
        RunAllTests
    }
}
