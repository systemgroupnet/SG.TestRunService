using SG.TestRunService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SG.TestRunService.Common.Models
{
    public static class ModelMappingExtensions
    {
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
                Azure_ProductBuildDefinitionId = r.Azure_ProductBuildDefinitionId,
                Azure_ProductBuildId = r.Azure_ProductBuildId,
                Azure_TestBuildId = r.Azure_TestBuildId,
                Azure_ProductBuildNumber = r.Azure_ProductBuildNumber,
                Azure_TestBuildNumber = r.Azure_TestBuildNumber,
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
                Azure_TestCaseId = r.Azure_TestCaseId,
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
                ExtraData = testRun.ExtraData.ToDictionary(
                    e => e.Name,
                    e => new ExtraDataValue()
                        {
                            Value = e.Value,
                            Url = e.Url
                        })
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
                Azure_ProductBuildDefinitionId = session.Azure_ProductBuildDefinitionId,
                Azure_ProductBuildId = session.Azure_ProductBuildId,
                Azure_TestBuildId = session.Azure_TestBuildId,
                Azure_ProductBuildNumber = session.Azure_ProductBuildNumber,
                Azure_TestBuildNumber = session.Azure_TestBuildNumber,
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
                    Azure_ProductBuildDefinitionId = session.Azure_ProductBuildDefinitionId,
                    Azure_ProductBuildId = session.Azure_ProductBuildId,
                    Azure_TestBuildId = session.Azure_TestBuildId,
                    Azure_ProductBuildNumber = session.Azure_ProductBuildNumber,
                    Azure_TestBuildNumber = session.Azure_TestBuildNumber,
                    SuiteName = session.SuiteName,
                    SourceVersion = session.SourceVersion,
                    StartTime = session.StartTime,
                    FinishTime = session.FinishTime,
                    Outcome = session.Outcome
                });
        }
    }
}
