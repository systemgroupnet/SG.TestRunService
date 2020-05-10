using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SG.TestRunService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.IntegrationTests
{
    public class TestingWebAppFactory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                      d => d.ServiceType ==
                         typeof(DbContextOptions<TSDbContext>));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<TSDbContext>(options =>
                        options.UseInMemoryDatabase("InMemoryTestRunServiceDb"));

                    var sp = services.BuildServiceProvider();
                    using (var scope = sp.CreateScope())
                    {
                        using (var db = scope.ServiceProvider.GetRequiredService<TSDbContext>())
                        {
                            db.Database.EnsureCreated();
                        }
                    }
                });
        }
    }
}
