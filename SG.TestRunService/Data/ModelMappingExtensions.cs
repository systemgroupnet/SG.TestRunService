using SG.TestRunService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SG.TestRunService.Common.Models
{
    public static class ModelMappingExtensions
    {
        public static IList<TTarget> ConvertAll<TSource, TTarget>(this IList<TSource> source, Func<TSource, TTarget> converter)
        {
            List<TTarget> result = new List<TTarget>(source.Count);
            foreach (var s in source)
                result.Add(converter(s));
            return result;
        }

        public static IList<ExtraData> ToDataModel(this IDictionary<string, ExtraDataValue> extraData)
        {
            return extraData
                .Select(
                    e => new Data.ExtraData()
                    {
                        Name = e.Key,
                        Value = e.Value.Value,
                        Url = e.Value.Url
                    }).ToList();
        }

        public static TestRunSession ToDataModel(this TestRunSessionRequest r)
        {
            return new TestRunSession()
            {
                TeamProject = r.TeamProject,
                AzureProductBuildDefinitionId = r.AzureProductBuildDefinitionId,
                AzureProductBuildId = r.AzureProductBuildId,
                AzureTestBuildId = r.AzureTestBuildId,
                AzureProductBuildNumber = r.AzureProductBuildNumber,
                AzureTestBuildNumber = r.AzureTestBuildNumber,
                SourceVersion = r.SourceVersion,
                StartTime = r.StartTime,
                FinishTime = r.FinishTime,
                Outcome = r.Outcome,
                TestRuns = r.TestRuns.ConvertAll(tr => tr.ToDataModel())
            };
        }

        public static TestCase ToDataModel(this TestCaseRequest r)
        {
            return new TestCase()
            {
                AzureTestCaseId = r.AzureTestCaseId,
                TeamProject = r.TeamProject,
                Title = r.Title,
                ExtraData = r.ExtraData.ToDataModel()
            };
        }

        public static TestRun ToDataModel(this TestRunRequest r, int? testRunSessionId = null)
        {
            return new TestRun()
            {
                TestRunSessionId = testRunSessionId ?? 0,
                TestCaseId = r.TestCaseId,
                Outcome = r.Outcome,
                StartTime = r.StartTime,
                FinishTime = r.FinishTime,
                ExtraData = r.ExtraData.ToDataModel()
            };
        }

        public static TestRunResponse ToResponse(this TestRun testRun)
        {
            return new TestRunResponse()
            {
                Id = testRun.Id,
                TestCaseId = testRun.TestCaseId,
                TestRunSessionId = testRun.TestRunSessionId,
                Outcome = testRun.Outcome,
                StartTime = testRun.StartTime,
                FinishTime = testRun.FinishTime,
                ExtraData = testRun.ExtraData.ToResponse()
            };
        }

        public static IQueryable<TestRunResponse> Project(this IQueryable<TestRun> runs)
        {
            return runs.Select(
                r => new TestRunResponse()
                {
                    Id = r.Id,
                    TestCaseId = r.TestCaseId,
                    TestRunSessionId = r.TestRunSessionId,
                    Outcome = r.Outcome,
                    StartTime = r.StartTime,
                    FinishTime = r.FinishTime
                });
        }

        public static TestRunSessionResponse ToResponse(this TestRunSession session)
        {
            return new TestRunSessionResponse()
            {
                Id = session.Id,
                TeamProject = session.TeamProject,
                AzureProductBuildDefinitionId = session.AzureProductBuildDefinitionId,
                AzureProductBuildId = session.AzureProductBuildId,
                AzureTestBuildId = session.AzureTestBuildId,
                AzureProductBuildNumber = session.AzureProductBuildNumber,
                AzureTestBuildNumber = session.AzureTestBuildNumber,
                SuiteName = session.SuiteName,
                SourceVersion = session.SourceVersion,
                StartTime = session.StartTime,
                FinishTime = session.FinishTime,
                Outcome = session.Outcome
            };
        }

        public static IQueryable<TestRunSessionResponse> Project(this IQueryable<TestRunSession> sessions)
        {
            return sessions.Select(
                session => new TestRunSessionResponse()
                {
                    Id = session.Id,
                    TeamProject = session.TeamProject,
                    AzureProductBuildDefinitionId = session.AzureProductBuildDefinitionId,
                    AzureProductBuildId = session.AzureProductBuildId,
                    AzureTestBuildId = session.AzureTestBuildId,
                    AzureProductBuildNumber = session.AzureProductBuildNumber,
                    AzureTestBuildNumber = session.AzureTestBuildNumber,
                    SuiteName = session.SuiteName,
                    SourceVersion = session.SourceVersion,
                    StartTime = session.StartTime,
                    FinishTime = session.FinishTime,
                    Outcome = session.Outcome
                });
        }

        public static IQueryable<TestCaseResponse> Project(this IQueryable<TestCase> testCases)
        {
            return testCases.Select(
                testCase => new TestCaseResponse()
                {
                    Id = testCase.Id,
                    AzureTestCaseId = testCase.AzureTestCaseId,
                    TeamProject = testCase.TeamProject,
                    Title = testCase.Title,
                });
        }

        public static IDictionary<string, ExtraDataValue> ToResponse(this IEnumerable<ExtraData> extraData)
        {
            return extraData.ToDictionary(
                e => e.Name,
                e => new ExtraDataValue()
                {
                    Value = e.Value,
                    Url = e.Url
                });
        }

        public static TestCaseResponse ToResponse(this TestCase testCase)
        {
            return new TestCaseResponse()
            {
                Id = testCase.Id,
                AzureTestCaseId = testCase.AzureTestCaseId,
                TeamProject = testCase.TeamProject,
                Title = testCase.Title,
                ExtraData = testCase.ExtraData.ToResponse()
            };
        }
    }
}
