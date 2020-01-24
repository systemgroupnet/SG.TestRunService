using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.Controllers
{
    public static class RoutConstants
    {
        public const string ApiRoot = "api";
        public const string TestRuns = ApiRoot + "/runs";
        public const string Sessions = ApiRoot + "/sessions";
        public const string Impact = ApiRoot + "/impact";
        public static string GetSessionTestRuns(int sessionId) => $"{Sessions}/{sessionId}/runs";
        public static string GetSessionTestRun(int sessionId, int runId) => $"{GetSessionTestRuns(sessionId)}/{runId}";
        public const string TestCases = ApiRoot + "/testcases";
    }
}
