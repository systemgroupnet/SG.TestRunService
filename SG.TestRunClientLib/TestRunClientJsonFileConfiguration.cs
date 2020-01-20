using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace SG.TestRunClientLib
{
    public class TestRunClientJsonFileConfiguration : ITestRunClientConfiguration
    {
        private readonly IConfigurationRoot _configuration;

        private static IConfigurationRoot BuildConfiguration(string jsonConfigPath)
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddJsonFile(jsonConfigPath);
            return builder.Build();
        }

        public TestRunClientJsonFileConfiguration(string jsonConfigPath)
        {
            _configuration = BuildConfiguration(jsonConfigPath);
        }

        public string TestRunServiceUrl => _configuration["testRunServiceUri"];

        public TestOlderVersionBehavior RunForOlderVersionBeahvior =>
            _configuration.GetValue(
                "runForOlderVersionBehavior",
                TestOlderVersionBehavior.RunImpactedAndNotSuccessfulTests);
    }
}
