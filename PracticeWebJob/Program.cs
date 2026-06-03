using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace PracticeWebJob
{
    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?linkid=2250384
    internal class Program
    {
        // Please set AzureWebJobsStorage connection strings in appsettings.json for this WebJob to run.
        public static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                })
                .ConfigureWebJobs(b =>
                {
                    b.AddAzureStorageCoreServices()
                    .AddAzureStorageQueues();
                })
                .ConfigureLogging((context, b) =>
                {
                    b.SetMinimumLevel(LogLevel.Information);
                    b.AddConsole();



                    string AppInsightsConnectionString = context.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];
                    if (!string.IsNullOrEmpty(AppInsightsConnectionString))
                    {   // add appinsights
                        b.AddApplicationInsightsWebJobs(o => o.ConnectionString = AppInsightsConnectionString);
                    }
                });


            var host = builder.Build();
            using (host)
            {
                var config = host.Services.GetService<IConfiguration>();

                Console.WriteLine("*****************************************************************************************");
                Console.WriteLine("Configurations Loaded:\n   Connection String: {0}\n   Storage: {1}\n   Insights: {2}\n   webjobq: {3} ",
                    config.GetConnectionString("DefaultConnection"),
                config["AzureWebJobsStorage"],
                config["APPLICATIONINSIGHTS_CONNECTION_STRING"],
                config["webjobq"]);
                Console.WriteLine("*****************************************************************************************");

                // The following code ensures that the WebJob will be running continuously

                await host.RunAsync();
            }
        }
    }
}


