using Newtonsoft.Json;
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
        private readonly BuildInfo _build;
        private readonly ILogger _logger;

        private static readonly JsonSerializerSettings _logSerializerSettings = CreateLogSerializerSettings();

        public TestStateUpdater(TestRunClient client, IDevOpsServerHandle devOpsServerHandle, BuildInfo build, ILogger logger = null)
        {
            _client = client;
            _devOpsServerHandle = devOpsServerHandle;
            _build = build;
            _logger = logger ?? new NullLogger();
        }

        public async Task<PublishImpactChangesResponse> PublishChanges()
        {
            var currentSourceVersion = _build.SourceVersion;
            var baseBuild = await GetBaseBuildAsync();
            var baseSourceVersion = baseBuild.SourceVersion;
            if (baseSourceVersion == null)
            {
                _logger.Info("This is the first test session for pipeline " + _build.AzureBuildDefinitionId + ". All tests will be run.");
                return await PublishNoBaseBuild();
            }
            if (baseSourceVersion == currentSourceVersion)
            {
                _logger.Warn("This test session is being run for the same source version as the previous session: " + currentSourceVersion);
                return await PublishChanges(Enumerable.Empty<string>());
            }
            else if (!_devOpsServerHandle.IsChronologicallyAfter(currentSourceVersion, baseSourceVersion))
            {
                _logger.Info($"Source version used for this test ({currentSourceVersion}) is older than the last test ran on this build definition ({baseSourceVersion}). Nothing to update.");
                return PublishImpactChangesResponse.Empty();
            }
            else
            {
                var changedFiles = _devOpsServerHandle.GetBuildChanges(baseBuild, _build);
                LogChangedFiles(currentSourceVersion, baseSourceVersion, changedFiles);
                return await PublishChanges(changedFiles);
            }
        }

        public async Task<PublishImpactChangesResponse> PublishNoBaseBuild()
        {
            PublishImpactChangesRequest req = new PublishImpactChangesRequest()
            {
                AzureProductBuildDefinitionId = _build.AzureBuildDefinitionId,
                ProductBuild = _build,
                CodeSignatures = new List<string>(),
                NoBaseBuild = true
            };
            return await PublishChanges(req);
        }

        private async Task<PublishImpactChangesResponse> PublishChanges(IEnumerable<string> changedFilesOrMethods)
        {
            PublishImpactChangesRequest req = new PublishImpactChangesRequest()
            {
                AzureProductBuildDefinitionId = _build.AzureBuildDefinitionId,
                ProductBuild = _build,
                CodeSignatures = changedFilesOrMethods.Select(CodeSignatureUtils.CalculateSignature).ToList()
            };
            return await PublishChanges(req);
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
            var lastUpdate = await _client.GetLastImpactUpdateAsync(_build.AzureBuildDefinitionId);
            if (lastUpdate == null)
            {
                _logger.Debug("No previous impact info is available for pipeline " + _build.AzureBuildDefinitionId);
                return null;
            }
            LogDebug("Previous update information:", lastUpdate);
            return lastUpdate.ProductBuild;
        }

        private void LogChangedFiles(string currentSourceVersion, string baseSourceVersion, IReadOnlyList<string> changedFiles)
        {
            if (_logger.IsEnabled)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Changed files between source version {baseSourceVersion} and {currentSourceVersion}:");
                sb.AppendLine("===============================================================================");
                foreach (var c in changedFiles)
                {
                    sb.AppendLine(c);
                }
                sb.AppendLine("===============================================================================");
                _logger.Info(sb.ToString());
                _logger.Info("Total changed files: " + changedFiles.Count);
            }
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
