using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YoutubeChannelAPI.Data;

namespace YoutubeChannelAPITest;

public class IntegrationTestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var envConStr = Environment.GetEnvironmentVariable("GITLAB_TEST_CONNECTION_STRING");
        var configuration =
            new ConfigurationBuilder()
                .AddUserSecrets(Assembly.GetExecutingAssembly())
                .AddEnvironmentVariables()
                .Build();
        var connectionString = envConStr ?? configuration["Postgres:ConnectionString"];

        builder.ConfigureTestServices(services =>
        {
            services.RemoveDbContext<YoutubeChannelContext>();

            services.AddNpgsql<YoutubeChannelContext>(connectionString);
            
            services.EnsureDbCreated<YoutubeChannelContext>();
        });
    }
}