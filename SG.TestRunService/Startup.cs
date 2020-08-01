using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using SG.TestRunService.Data;
using SG.TestRunService.Data.Services;
using SG.TestRunService.Data.Services.Implementations;
using SG.TestRunService.ServiceImplementations;
using SG.TestRunService.Services;
using System;

namespace SG.TestRunService
{
    public class Startup
    {
        private const string corsAllowSgServersPolicyName = "sgServers";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddProblemDetails(options =>
            {
                options.IncludeExceptionDetails = (ctcx, ex) => true;
                options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
            });

            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                });

            services.AddDbContext<TSDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("db")));

            services.AddCors(options =>
            {
                options.AddPolicy(name: corsAllowSgServersPolicyName,
                                  builder => builder.WithOrigins(new string[] {
                                      "http://ws-702",
                                      "https://ws-702",
                                      "http://ws-702:3000",
                                      "https://ws-702:3000",
                                      "http://srv-framework",
                                      "https://srv-framework",
                                      "http://alborzscm",
                                      "https://alborzscm",
                                      "http://alborzscm:8080",
                                      "https://alborzscm:8080"
                                  })
                                  .AllowAnyMethod()
                                  .AllowAnyHeader());
            });

            services.AddTransient<IBaseDbService, BaseDbService>();
            services.AddTransient<ITestRunSessionService, TestRunSessionService>();
            services.AddTransient<ITestCaseService, TestCaseService>();
            services.AddTransient<ITestImpactService, TestImpactService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseProblemDetails();
            app.UseRouting();
            app.UseSerilogRequestLogging();
            app.UseCors(corsAllowSgServersPolicyName);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
