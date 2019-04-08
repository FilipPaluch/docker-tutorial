using System;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using DockerTutorial.Infrastructure.Config;
using DockerTutorial.Infrastructure.Database;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;

namespace DockerTutorial
{
    class Program
    {
        private static readonly ILogger Logger = Log.ForContext<Program>();


        public static async Task Main(string[] args)
        {
            try
            {
                var config = Configuration.Read();
                
                ConfigureLogger(config);

                await DatabaseCreator.CreateDatabaseAndTableIfNotExists(config.GetSection("Database").Get<DatabaseConfig>());
                
                WebHost.CreateDefaultBuilder()
                    .ConfigureServices(services =>
                    {
                        services.AddAutofac();
                        services.AddSingleton(config);
                    })
                    .UseStartup<Startup>()
                    .UseUrls($"http://*:5000")
                    .UseSerilog()
                    .Build()
                    .Run();
            }
            catch (Exception exception)
            {
                Logger.Fatal(exception, "DockerTutorialAPI error on startup");
                throw;
            }
            finally
            {
                Logger.Information("Stopping DockerTutorialAPI");
                Log.CloseAndFlush();
            }

        }

        private static void ConfigureLogger(IConfiguration config)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();
        }
    }
}
