using SG.TestRunService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SG.TestRunService.Common.Models
{
    public static class ModelMappingExtensions
    {
        #region Helpers

        public static IList<TTarget> ConvertAll<TSource, TTarget>(this IList<TSource> source, Func<TSource, TTarget> converter)
        {
            if (source == null)
                return null;

            List<TTarget> result = new List<TTarget>(source.Count);
            foreach (var s in source)
                result.Add(converter(s));
            return result;
        }

        #endregion Helpers
        #region ExtraData

        public static IList<ExtraData> ToDataModel(this IDictionary<string, ExtraDataValue> extraData)
        {
            if (extraData == null)
                return null;

            return extraData
                .Select(
                    e => new Data.ExtraData()
                    {
                        Name = e.Key,
                        Value = e.Value.Value,
                        Url = e.Value.Url
                    }).ToList();
        }

        public static IDictionary<string, ExtraDataValue> ToDto(this IEnumerable<ExtraData> extraData)
        {
            if (extraData == null)
                return null;

            return extraData.ToDictionary(
                e => e.Name,
                e => new ExtraDataValue()
                {
                    Value = e.Value,
                    Url = e.Url
                });
        }

        public static void UpdateFrom(this IList<ExtraData> extraData, IDictionary<string, ExtraDataValue> extraDataRequest)
        {
            var extraDataDict = extraData.ToDictionary(e => e.Value);
            foreach(var requestItem in extraDataRequest)
            {
                if(extraDataDict.TryGetValue(requestItem.Key, out var dataItem))
                {
                    dataItem.Value = requestItem.Value.Value;
                    dataItem.Url = requestItem.Value.Url;
                }
                else
                {
                    extraData.Add(
                        new ExtraData()
                        {
                            Name = requestItem.Key,
                            Value = requestItem.Value.Value,
                            Url = requestItem.Value.Url
                        });
                }
            }
            var toRemove = extraData.Where(d => !extraDataRequest.ContainsKey(d.Name));
            foreach (var d in toRemove)
                extraData.Remove(d);
        }

        #endregion ExtraData
        #region BuildInfo

        public static Models.BuildInfo ToResponse(this Data.BuildInfo buildInfo)
        {
            return new Models.BuildInfo()
            {
                TeamProject = buildInfo.TeamProject,
                AzureBuildDefinitionId = buildInfo.AzureBuildDefinitionId,
                AzureBuildId = buildInfo.AzureBuildId,
                SourceVersion = buildInfo.SourceVersion,
                Date = buildInfo.Date,
                BuildNumber = buildInfo.BuildNumber,
            };
        }

        public static Data.BuildInfo ToDataModel(this Models.BuildInfo buildInfo)
        {
            return new Data.BuildInfo()
            {
                TeamProject = buildInfo.TeamProject,
                AzureBuildDefinitionId = buildInfo.AzureBuildDefinitionId,
                AzureBuildId = buildInfo.AzureBuildId,
                SourceVersion = buildInfo.SourceVersion,
                Date = buildInfo.Date,
                BuildNumber = buildInfo.BuildNumber,
            };
        }

        public static IQueryable<Models.BuildInfo> Project(this IQueryable<Data.BuildInfo> query)
        {
            return query.Select(x => new BuildInfo()
            {
                TeamProject = x.TeamProject,
                AzureBuildDefinitionId = x.AzureBuildDefinitionId,
                AzureBuildId = x.AzureBuildId,
                SourceVersion = x.SourceVersion,
                Date = x.Date,
                BuildNumber = x.BuildNumber,
            });
        }

        #endregion BuildInfo
        #region TestRunSession

        public static TestRunSession ToDataModel(this TestRunSessionRequest r)
        {
            return new TestRunSession()
            {
                ProductBuildInfo = r.ProductBuild.ToDataModel(),
                AzureTestBuildId = r.AzureTestBuildId,
                AzureTestBuildNumber = r.AzureTestBuildNumber,
                SuiteName = r.SuiteName,
                StartTime = r.StartTime,
                FinishTime = r.FinishTime,
                State = r.State,
                TestRuns = r.TestRuns.ConvertAll(tr => tr.ToDataModel())
            };
        }

        public static TestRunSessionResponse ToResponse(this TestRunSession session)
        {
            return new TestRunSessionResponse()
            {
                Id = session.Id,
                ProductBuild = session.ProductBuildInfo.ToResponse(),
                AzureTestBuildId = session.AzureTestBuildId,
                AzureTestBuildNumber = session.AzureTestBuildNumber,
                SuiteName = session.SuiteName,
                StartTime = session.StartTime,
                FinishTime = session.FinishTime,
                State = session.State
            };
        }

        public static TestRunSessionRequest ToRequest(this TestRunSession session)
        {
            return new TestRunSessionRequest()
            {
                AzureTestBuildId = session.AzureTestBuildId,
                AzureTestBuildNumber = session.AzureTestBuildNumber,
                SuiteName = session.SuiteName,
                StartTime = session.StartTime,
                FinishTime = session.FinishTime,
                State = session.State,
                TestRuns = session.TestRuns.Select(ToRequest).ToList()
            };
        }

        public static void Update(this TestRunSessionRequest request, TestRunSession session)
        {
            session.AzureTestBuildId = request.AzureTestBuildId;
            session.AzureTestBuildNumber = request.AzureTestBuildNumber;
            session.SuiteName = request.SuiteName;
            session.StartTime = request.StartTime;
            session.FinishTime = request.FinishTime;
            session.State = request.State;
        }

        public static IQueryable<TestRunSessionResponse> Project(this IQueryable<TestRunSession> sessions)
        {
            return sessions.Select(
                session => new TestRunSessionResponse()
                {
                    Id = session.Id,
                    ProductBuild = new BuildInfo()
                    {
                        TeamProject = session.ProductBuildInfo.TeamProject,
                        AzureBuildDefinitionId = session.ProductBuildInfo.AzureBuildDefinitionId,
                        AzureBuildId = session.ProductBuildInfo.AzureBuildId,
                        SourceVersion = session.ProductBuildInfo.SourceVersion,
                        Date = session.ProductBuildInfo.Date,
                        BuildNumber = session.ProductBuildInfo.BuildNumber,
                    },
                    AzureTestBuildId = session.AzureTestBuildId,
                    AzureTestBuildNumber = session.AzureTestBuildNumber,
                    SuiteName = session.SuiteName,
                    StartTime = session.StartTime,
                    FinishTime = session.FinishTime,
                    State = session.State
                });
        }

        #endregion TestRunSession
        #region TestCase

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

        public static TestCaseResponse ToResponse(this TestCase testCase)
        {
            return new TestCaseResponse()
            {
                Id = testCase.Id,
                AzureTestCaseId = testCase.AzureTestCaseId,
                TeamProject = testCase.TeamProject,
                Title = testCase.Title,
                ExtraData = testCase.ExtraData.ToDto()
            };
        }

        #endregion TestCase
        #region TestRun

        public static TestRun ToDataModel(this TestRunRequest r, int? testRunSessionId = null)
        {
            return new TestRun()
            {
                TestRunSessionId = testRunSessionId ?? 0,
                TestCaseId = r.TestCaseId,
                State = r.State,
                StartTime = r.StartTime,
                FinishTime = r.FinishTime,
                Outcome = r.Outcome,
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
                State = testRun.State,
                StartTime = testRun.StartTime,
                FinishTime = testRun.FinishTime,
                Outcome = testRun.Outcome,
                ExtraData = testRun.ExtraData.ToDto()
            };
        }

        public static TestRunRequest ToRequest(this TestRun testRun)
        {
            return new TestRunRequest()
            {
                TestCaseId = testRun.TestCaseId,
                State = testRun.State,
                StartTime = testRun.StartTime,
                FinishTime = testRun.FinishTime,
                Outcome = testRun.Outcome,
                ExtraData = testRun.ExtraData.ToDto()
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

        public static void Update(this TestRunRequest testRunRequest, TestRun testRun)
        {
            testRun.TestCaseId = testRunRequest.TestCaseId;
            testRun.Outcome = testRunRequest.Outcome;
            testRun.StartTime = testRunRequest.StartTime;
            testRun.FinishTime = testRunRequest.FinishTime;
            testRun.ExtraData.UpdateFrom(testRunRequest.ExtraData);
        }

        #endregion
        #region LastImpactUpdate

        public static IQueryable<LastImpactUpdateResponse> Project(this IQueryable<LastImpactUpdate> query)
        {
            return query.Select(
                l => new LastImpactUpdateResponse()
                {
                    AzureProductBuildDefinitionId = l.AzureProductBuildDefinitionId,
                    UpdateDate = l.UpdateDate,
                    ProductBuild = new BuildInfo()
                    {
                        TeamProject = l.ProductBuildInfo.TeamProject,
                        AzureBuildDefinitionId = l.ProductBuildInfo.AzureBuildDefinitionId,
                        AzureBuildId = l.ProductBuildInfo.AzureBuildId,
                        SourceVersion = l.ProductBuildInfo.SourceVersion,
                        Date = l.ProductBuildInfo.Date,
                        BuildNumber = l.ProductBuildInfo.BuildNumber,
                    },
                });
        }

        #endregion
        #region TestLastState

        public static IQueryable<TestLastStateResponse> Project(this IQueryable<TestLastState> lastStates)
        {
            return lastStates.Select(l => new TestLastStateResponse()
            {
                TestCaseId = l.TestCaseId,
                AzureProductBuildDefinitionId = l.AzureProductBuildDefinitionId,
                ProductBuildInfo = new BuildInfo()
                {
                    TeamProject = l.ProductBuildInfo.TeamProject,
                    AzureBuildDefinitionId = l.ProductBuildInfo.AzureBuildDefinitionId,
                    AzureBuildId = l.ProductBuildInfo.AzureBuildId,
                    SourceVersion = l.ProductBuildInfo.SourceVersion,
                    Date = l.ProductBuildInfo.Date,
                    BuildNumber = l.ProductBuildInfo.BuildNumber
                },
                UpdateDate = l.UpdateDate,
                LastOutcome = l.LastOutcome,
                ShouldBeRun = l.ShouldBeRun,
                RunReason = l.RunReason
            });
        }

        #endregion
    }
}
