using SG.TestRunService.Common.Models;
using SG.TestRunService.Data.FetchModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SG.TestRunService.Data
{
    internal static class ModelMappingExtensions
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
                    e => new ExtraData()
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
            var extraDataDict = extraData.ToDictionary(e => e.Name);
            foreach (var requestItem in extraDataRequest)
            {
                if (extraDataDict.TryGetValue(requestItem.Key, out var dataItem))
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
            var toRemove = extraData.Where(d => !extraDataRequest.ContainsKey(d.Name)).ToList();
            foreach (var d in toRemove)
                extraData.Remove(d);
        }

        #endregion ExtraData
        #region BuildInfo

        public static Common.Models.BuildInfo ToDto(this BuildInfo buildInfo)
        {
            return new Common.Models.BuildInfo()
            {
                TeamProject = buildInfo.TeamProject,
                AzureBuildDefinitionId = buildInfo.AzureBuildDefinitionId,
                AzureBuildId = buildInfo.AzureBuildId,
                SourceVersion = buildInfo.SourceVersion,
                Date = buildInfo.Date,
                BuildNumber = buildInfo.BuildNumber,
            };
        }

        public static BuildInfo ToDataModel(this Common.Models.BuildInfo buildInfo)
        {
            return new BuildInfo()
            {
                TeamProject = buildInfo.TeamProject,
                AzureBuildDefinitionId = buildInfo.AzureBuildDefinitionId,
                AzureBuildId = buildInfo.AzureBuildId,
                SourceVersion = buildInfo.SourceVersion,
                Date = buildInfo.Date,
                BuildNumber = buildInfo.BuildNumber,
            };
        }

        public static IQueryable<Common.Models.BuildInfo> Project(this IQueryable<BuildInfo> query)
        {
            return query.Select(x => new Common.Models.BuildInfo()
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
        #region ProductLine

        public static Common.Models.ProductLine ToDto(this ProductLine productLine)
        {
            return new Common.Models.ProductLine()
            {
                Id = productLine.Id,
                Key = productLine.Key,
                AzureProductBuildDefinitionId = productLine.AzureProductBuildDefinitionId
            };
        }

        #endregion
        #region TestRunSession

        public static TestRunSession ToDataModel(this TestRunSessionRequest r)
        {
            if (r.ProductLine.Id == null)
                throw new ArgumentException("`ProductLine.Id` is required.");
            return new TestRunSession()
            {
                ProductLineId = r.ProductLine.Id.Value,
                ProductBuildInfo = r.ProductBuild.ToDataModel(),
                AzureTestBuildId = r.AzureTestBuildId,
                AzureTestBuildNumber = r.AzureTestBuildNumber,
                SuiteName = r.SuiteName,
                StartTime = r.StartTime,
                FinishTime = r.FinishTime,
                State = r.State,
                TestRuns = r.TestRuns.ConvertAll(tr => tr.ToDataModel()),
                ExtraData = r.ExtraData.ToDataModel()
            };
        }

        public static TestRunSessionResponse ToResponse(this TestRunSession session)
        {
            return new TestRunSessionResponse()
            {
                Id = session.Id,
                ProductLine = session.ProductLine != null
                    ? session.ProductLine.ToDto()
                    : new Common.Models.ProductLine() { Id = session.ProductLineId },
                ProductBuild = session.ProductBuildInfo?.ToDto(),
                AzureTestBuildId = session.AzureTestBuildId,
                AzureTestBuildNumber = session.AzureTestBuildNumber,
                SuiteName = session.SuiteName,
                StartTime = session.StartTime,
                FinishTime = session.FinishTime,
                State = session.State,
                ExtraData = session.ExtraData.ToDto()
            };
        }

        public static TestRunSessionRequest ToRequest(this TestRunSession session)
        {
            return new TestRunSessionRequest()
            {
                ProductLine = session.ProductLine != null
                    ? session.ProductLine.ToDto()
                    : new Common.Models.ProductLine() { Id = session.ProductLineId },
                AzureTestBuildId = session.AzureTestBuildId,
                AzureTestBuildNumber = session.AzureTestBuildNumber,
                SuiteName = session.SuiteName,
                StartTime = session.StartTime,
                FinishTime = session.FinishTime,
                State = session.State,
                ProductBuild = session.ProductBuildInfo?.ToDto(),
                TestRuns = session.TestRuns?.Select(ToRequest).ToList(),
                ExtraData = session.ExtraData.ToDto()
            };
        }

        public static void Update(this TestRunSessionRequest request, TestRunSession session)
        {
            if (request.ProductLine.Id.HasValue)
                session.ProductLineId = request.ProductLine.Id.Value;
            session.AzureTestBuildId = request.AzureTestBuildId;
            session.AzureTestBuildNumber = request.AzureTestBuildNumber;
            session.SuiteName = request.SuiteName;
            session.StartTime = request.StartTime;
            session.FinishTime = request.FinishTime;
            session.State = request.State;
            session.ExtraData.UpdateFrom(request.ExtraData);
        }

        public static IQueryable<FTestRunSession> Project(this IQueryable<TestRunSession> sessions)
        {
            return sessions.Select(
                session => new FTestRunSession()
                {
                    Response = new TestRunSessionResponse()
                    {
                        Id = session.Id,
                        ProductLine = new Common.Models.ProductLine()
                        {
                            Id = session.ProductLineId,
                            Key = session.ProductLine.Key,
                            AzureProductBuildDefinitionId = session.ProductLine.AzureProductBuildDefinitionId
                        },
                        ProductBuild = new Common.Models.BuildInfo()
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
                        State = session.State,
                        TestRunCount = session.TestRuns.Count()
                    },
                    ExtraData = session.ExtraData
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
                ErrorMessage = r.ErrorMessage,
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
                ErrorMessage = testRun.ErrorMessage,
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
                ErrorMessage = testRun.ErrorMessage,
                ExtraData = testRun.ExtraData.ToDto()
            };
        }

        public static IQueryable<FTestRun> Project(this IQueryable<TestRun> runs)
        {
            return runs.Select(
                r => new FTestRun
                {
                    Response = new TestRunResponse()
                    {
                        Id = r.Id,
                        TestCaseId = r.TestCaseId,
                        TestCase = new TestCaseResponse
                        {
                            Id = r.TestCase.Id,
                            AzureTestCaseId = r.TestCase.AzureTestCaseId,
                            Title = r.TestCase.Title,
                            TeamProject = r.TestCase.TeamProject
                        },
                        TestRunSessionId = r.TestRunSessionId,
                        State = r.State,
                        Outcome = r.Outcome,
                        StartTime = r.StartTime,
                        FinishTime = r.FinishTime,
                        ErrorMessage = r.ErrorMessage
                    },
                    TestRunExtraData = r.ExtraData,
                    TestCaseExtraData = r.TestCase.ExtraData
                });
        }

        public static void Update(this TestRunRequest testRunRequest, TestRun testRun)
        {
            testRun.TestCaseId = testRunRequest.TestCaseId;
            testRun.State = testRunRequest.State;
            testRun.StartTime = testRunRequest.StartTime;
            testRun.FinishTime = testRunRequest.FinishTime;
            testRun.Outcome = testRunRequest.Outcome;
            testRun.ErrorMessage = testRunRequest.ErrorMessage;
            testRun.ExtraData.UpdateFrom(testRunRequest.ExtraData);
        }

        #endregion
        #region LastImpactUpdate

        public static IQueryable<LastImpactUpdateResponse> Project(this IQueryable<LastImpactUpdate> query)
        {
            return query.Select(
                l => new LastImpactUpdateResponse()
                {
                    ProductLine = new Common.Models.ProductLine()
                    {
                        Id = l.ProductLineId,
                        Key = l.ProductLine.Key,
                        AzureProductBuildDefinitionId = l.ProductLine.AzureProductBuildDefinitionId
                    },
                    UpdateDate = l.UpdateDate,
                    ProductBuild = new Common.Models.BuildInfo()
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

        public static LastImpactUpdateResponse ToResponse(this LastImpactUpdate lastImpactUpdate)
        {
            var response = new LastImpactUpdateResponse()
            {
                ProductLine = lastImpactUpdate.ProductLine != null
                    ? lastImpactUpdate.ProductLine.ToDto()
                    : new Common.Models.ProductLine() { Id = lastImpactUpdate.ProductLineId },
                UpdateDate = lastImpactUpdate.UpdateDate,
                ProductBuild = lastImpactUpdate.ProductBuildInfo?.ToDto()
            };
            return response;
        }

        #endregion
        #region TestLastState

        public static IQueryable<TestLastStateResponse> Project(this IQueryable<TestLastState> lastStates)
        {
            return lastStates.Select(lastState => new TestLastStateResponse()
            {
                TestCaseId = lastState.TestCaseId,
                ProductLineId = lastState.ProductLineId,
                LastOutcome = lastState.LastOutcome,
                LastOutcomeProductBuildInfo = new Common.Models.BuildInfo()
                {
                    TeamProject = lastState.LastOutcomeProductBuildInfo.TeamProject,
                    AzureBuildDefinitionId = lastState.LastOutcomeProductBuildInfo.AzureBuildDefinitionId,
                    AzureBuildId = lastState.LastOutcomeProductBuildInfo.AzureBuildId,
                    SourceVersion = lastState.LastOutcomeProductBuildInfo.SourceVersion,
                    Date = lastState.LastOutcomeProductBuildInfo.Date,
                    BuildNumber = lastState.LastOutcomeProductBuildInfo.BuildNumber
                },
                LastOutcomeDate = lastState.LastOutcomeDate,
                LastImpactedProductBuildInfo = new Common.Models.BuildInfo()
                {
                    TeamProject = lastState.LastImpactedProductBuildInfo.TeamProject,
                    AzureBuildDefinitionId = lastState.LastImpactedProductBuildInfo.AzureBuildDefinitionId,
                    AzureBuildId = lastState.LastImpactedProductBuildInfo.AzureBuildId,
                    SourceVersion = lastState.LastImpactedProductBuildInfo.SourceVersion,
                    Date = lastState.LastImpactedProductBuildInfo.Date,
                    BuildNumber = lastState.LastImpactedProductBuildInfo.BuildNumber
                },
                LastImpactedDate = lastState.LastImpactedDate,
                ShouldBeRun = lastState.ShouldBeRun,
                RunReason = lastState.RunReason
            });
        }

        public static TestLastStateResponse ToResponse(this TestLastState testLastState)
        {
            return new TestLastStateResponse()
            {
                TestCaseId = testLastState.TestCaseId,
                ProductLineId = testLastState.ProductLineId,
                LastOutcome = testLastState.LastOutcome,
                LastOutcomeProductBuildInfo = testLastState.LastOutcomeProductBuildInfo.ToDto(),
                LastOutcomeDate = testLastState.LastOutcomeDate,
                LastImpactedProductBuildInfo = testLastState.LastImpactedProductBuildInfo?.ToDto(),
                LastImpactedDate = testLastState.LastImpactedDate,
                ShouldBeRun = testLastState.ShouldBeRun,
                RunReason = testLastState.RunReason
            };
        }

        #endregion
    }
}
