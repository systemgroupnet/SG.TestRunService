﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SG.TestRunService.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SG.TestRunClientLib
{
    public class TestStateUpdater
    {
        private readonly TestRunClient _client;
        private readonly IDevOpsServerHandle _devOpsServerHandle;
        private readonly string _productLineKey;
        private readonly ProductLine _productLine;
        private readonly BuildInfo _build;
        private readonly ILogger _logger;

        private static readonly JsonSerializerSettings _logSerializerSettings = CreateLogSerializerSettings();

        public TestStateUpdater(TestRunClient client, IDevOpsServerHandle devOpsServerHandle, string productLineKey, BuildInfo build, ILogger logger = null)
        {
            _client = client;
            _devOpsServerHandle = devOpsServerHandle;
            _productLineKey = productLineKey;
            _build = build;
            _logger = logger ?? new NullLogger();
            _productLine = new ProductLine()
            {
                Key = _productLineKey,
                AzureProductBuildDefinitionId = _build.AzureBuildDefinitionId
            };
        }

        public async Task<PublishImpactChangesResponse> PublishChanges()
        {
            var currentSourceVersion = _build.SourceVersion;
            var baseBuild = await GetBaseBuildAsync();
            if (baseBuild == null)
            {
                _logger.Info($"This is the first test session for pipeline {_build.AzureBuildDefinitionId}, product-line '{_productLineKey}'. All tests will be run.");
                return await PublishNoBaseBuild();
            }
            var baseSourceVersion = baseBuild.SourceVersion;
            if (baseSourceVersion == currentSourceVersion)
            {
                _logger.Warn("This test session is being run for the same source version as the previous session: " + currentSourceVersion);
            }
            else if (!_devOpsServerHandle.IsChronologicallyAfter(currentSourceVersion, baseSourceVersion))
            {
                _logger.Info($"Source version used for this test ({currentSourceVersion}) is older than the last test ran on this build definition ({baseSourceVersion}). Nothing to update.");
                return PublishImpactChangesResponse.Empty();
            }
            var changes = await _devOpsServerHandle.GetBuildChangesAsync(baseBuild, _build);
            LogChanges(baseBuild, _build, changes);
            return await PublishChanges(changes);
        }

        public async Task<PublishImpactChangesResponse> PublishNoBaseBuild()
        {
            PublishImpactChangesRequest req = new PublishImpactChangesRequest()
            {
                ProductLine = _productLine,
                ProductBuild = _build,
                CodeSignatures = new List<string>(),
                NoBaseBuild = true
            };
            return await PublishChanges(req);
        }

        private async Task<PublishImpactChangesResponse> PublishChanges(IEnumerable<string> changedFilesOrMethods)
        {
            var codeSignaturesDict = changedFilesOrMethods.Distinct().ToDictionary(CodeSignatureUtils.CalculateSignature);
            PublishImpactChangesRequest req = new PublishImpactChangesRequest()
            {
                ProductLine = _productLine,
                ProductBuild = _build,
                CodeSignatures = codeSignaturesDict.Keys
            };
            var response = await PublishChanges(req);
            LogImpactedTests(codeSignaturesDict, response);
            return response;
        }

        private async Task<PublishImpactChangesResponse> PublishChanges(PublishImpactChangesRequest req)
        {
            if (req.NoBaseBuild)
                LogDebug("No base build. All tests will be run next time.");
            else
                LogDebug("Publishing changes to the service and updating last states of tests...");

            var response = await _client.PublishImpactChangesAsync(req);

            return response;
        }

        private async Task<BuildInfo> GetBaseBuildAsync()
        {
            var lastUpdate = await _client.GetLastImpactUpdateAsync(_productLine);
            if (lastUpdate == null)
            {
                _logger.Debug("No previous impact info is available for pipeline " + _build.AzureBuildDefinitionId);
                return null;
            }
            LogDebug("Previous update information:", lastUpdate);
            return lastUpdate.ProductBuild;
        }

        private void LogChanges(BuildInfo baseBuild, BuildInfo currentBuild, IReadOnlyList<string> changes)
        {
            if (_logger.IsEnabled)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Changed files or methods between build {baseBuild.BuildNumber} (version {baseBuild.SourceVersion}) and {currentBuild.BuildNumber} (version {currentBuild.SourceVersion}):");
                sb.AppendLine("===============================================================================");
                foreach (var c in changes)
                {
                    sb.AppendLine(c);
                }
                sb.Append("===============================================================================");
                _logger.Info(sb.ToString());
                _logger.Info("Total changed files/methods: " + changes.Count + "\r\n");
            }
        }

        private void LogImpactedTests(Dictionary<string, string> codeSignaturesDict, PublishImpactChangesResponse response)
        {
            if (response.ImpactedTests.Count == 0)
            {
                _logger.Debug("No tests impacted.");
                return;
            }
            _logger.Debug(response.ImpactedTests.Count + " tests impacted.");

            StringBuilder msg = new StringBuilder();
            int delimCount = 15;
            var header = new StringBuilder()
                .Append('=', delimCount).Append("  Impact Information  ").Append('=', delimCount)
                .ToString();

            msg.AppendLine().AppendLine(header);

            var testCasesDict = response.ImpactedTests.ToDictionary(it => it.TestCaseId, it => it.AzureTestCaseId);
            foreach (var cs in response.CodeSignatureImpactedTestCaseIds)
            {
                var path = codeSignaturesDict[cs.Key];
                var azureTestCaseIds = cs.Value.Select(id => testCasesDict[id]).OrderBy(t => t).ToList();

                msg.AppendLine(
                    $"{cs.Key}  \"{path}\" - {azureTestCaseIds.Count} test cases:");
                AppendTestCases(msg, azureTestCaseIds);
            }
            msg.Append('=', header.Length).AppendLine();
            _logger.Debug(msg.ToString());
        }

        private void AppendTestCases(StringBuilder sb, IReadOnlyList<int> testCaseIds)
        {
            for (int i = 0; i < testCaseIds.Count; i++)
            {
                if (i > 0)
                {
                    if (i % 1000 == 0)
                        sb.AppendLine(",");
                    else
                        sb.Append(", ");
                }
                sb.Append(testCaseIds[i]);
            }
            sb.AppendLine();
        }

        private void LogDebug(string text, object obj = null)
        {
            if (_logger.IsEnabled)
            {
                _logger.Debug(text + (obj != null ? ObjToString(obj) : string.Empty));
            }
        }

        private static JsonSerializerSettings CreateLogSerializerSettings()
        {
            var settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };
            settings.Converters.Add(new StringEnumConverter());
            return settings;
        }

        private static string ObjToString(object obj)
        {
            return obj != null
                ? Environment.NewLine + JsonConvert.SerializeObject(obj, _logSerializerSettings)
                : string.Empty;
        }
    }
}
