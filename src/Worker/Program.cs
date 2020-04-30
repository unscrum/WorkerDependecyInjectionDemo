using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Worker.Services;

[assembly:InternalsVisibleTo("Worker.Test")]
namespace Worker
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            // Channel is explicitly configured to do flush on it later.
            var channel = new InMemoryChannel {SendingInterval = TimeSpan.FromSeconds(10)};

            try
            {
                await new HostBuilder()
                    .ConfigureHostConfiguration(configuration =>
                    {
                        configuration
                            .AddEnvironmentVariables("NETCORE_")
                            .AddCommandLine(args);
                    })
                    .ConfigureAppConfiguration((hostContext, configuration) =>
                    {
                        configuration
                            .AddJsonFile("appsettings.json", false, true)
                            .AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json",
                                false, true);
                        
                        //optionally add in azure app config by setting a conneciton string AppConfig
                        var appConfig = configuration.Build().GetConnectionString("AppConfig");
                        if(!string.IsNullOrWhiteSpace(appConfig))
                            configuration.AddAzureAppConfiguration(appConfig);

                    })
                    .ConfigureLogging((hostContext, logging) =>
                    {
                        var logConfig = hostContext.Configuration.GetSection("Logging");

                        logging
                            .AddConfiguration(logConfig)
                            .AddDebug()
                            .AddConsole()
                            .AddEventSourceLogger()
                            .AddApplicationInsights();
                    })
                    .ConfigureServices((hostContext, services) =>
                    {
                        services
                            .AddApplicationInsightsTelemetryWorkerService()
                            .Configure<TelemetryConfiguration>(telemetry =>
                            {
                                telemetry.TelemetryChannel = channel;
                                if(string.IsNullOrEmpty(telemetry.InstrumentationKey))
                                    throw new ArgumentNullException(nameof(telemetry.InstrumentationKey));
                            })
                            .AddSingleton<IPickANumber, PickANumber>()
                            .AddTransient<ILooper, Looper>()
                            .AddTransient<IMyTelemetry, MyTelemetry>()
                            .AddHostedService<Worker>();
                    })
                    .RunConsoleAsync();
            }
            finally
            {
                // Explicitly call Flush() followed by sleep is required in ConsoleApp Apps.
                // This is to ensure that even if application terminates, telemetry is sent to the back-end.
                channel.Flush();
                Thread.Sleep(5000);
            }
        }
    }
}