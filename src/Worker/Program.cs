using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Worker.Services;

namespace Worker
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            // Channel is explicitly configured to do flush on it later.
            var channel = new InMemoryChannel();
            channel.SendingInterval = TimeSpan.FromSeconds(10);

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
                    })
                    .ConfigureLogging((hostContext, logging) =>
                    {
                        logging
                            .AddConfiguration(hostContext.Configuration.GetSection("Logging"))
                            .AddConsole()
                            .AddDebug()
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