using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace SG.TestRunClientLib
{
    public static class TestRunClientFactory
    {
        static readonly IConfigurationRoot _configuration = BuildConfiguration();

        private static IConfigurationRoot BuildConfiguration()
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            return builder.Build();
        }

        private static string GetServiceUri()
        {
            return _configuration["testRunServiceUri"];
        }

        public static TestRunClient CreateClient()
        {
            return new TestRunClient(GetServiceUri());
        }
    }
}

