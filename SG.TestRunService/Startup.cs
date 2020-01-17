using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SG.TestRunService.Data;
using SG.TestRunService.Infrastructure;
using SG.TestRunService.Infrastructure.Implementations;
using SG.TestRunService.ServiceImplementations;
using SG.TestRunService.Services;

namespace SG.TestRunService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDbContext<TSDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("db")));

            services.AddTransient<IBaseDbService, BaseDbService>();
            services.AddTransient<ITestRunSessionService, TestRunSessionService>();
            services.AddTransient<ITestCaseService, TestCaseService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
