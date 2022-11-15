# Worker Dependency Injection Demo

.NET Core gives us really cool out of the box dependency injection, but its is really only wired up for you with ASP.NET.
This repo will show you two ways to wire it up for service workers.

## What kinds of services will this approach work for
- Console Apps
- Custom Services
- Linux Services
- AWS Lambdas
- Azure WebJobs
- Azure Functions
- Windows Services
- Service Fabric Actors

## Two Types
There are really only two types of workers in the list ablove
### Run Once
- Console apps
- AWS Lambdas
- Azure WebJobs
- Azure Functions
### Hosted Service
- Custom Services
- Linux Services
- Windows Services
- Service Fabric Actors
  
## What's in Common between the two?
The two share a lot of the same components
- Configuration Builder
- Service Collection / Service Provider
- Logger / Logging Builder
 
## Configuration Builder
1. Can be pull in many sources
- Environment Variables
- Command Line
- JSON
- XML
- Azure Key Vault
- Azure Application Configuration
- AWS Parameter Store
          
2. Can be built more than once allowing to use settings in the first build to create the second
        
        var configurationBuilder = new ConfigurationBuilder()
          .AddEnvironmentVariables("DOTNETCORE_")
        
        //get the environment settings
        var environment = configurationBuilder
          .Build()
          .GetValue<string>("Environment");
        
        //add config files per environment
        configurationBuilder
          .AddJsonFile($"consolesettings.{environment}.json", false, false);
        
        //build the configuration for the container
        var configuration = configurationBuilder
          .Build();
          
## Service Collection / Service Provider
Allows registering:
- interfaces with implementation 
- implementations
- factory methods

Allows for the lifetimes:
- Transient - new every time
- Scoped - new once per scope
- Singleton - only once

## Logging Builder
Allows multiple providers:
- Console
- Debugger
- Event Log
- File based
- Azure Monitor
- AWS Cloudwatch

Allows for levels:
- Trace
- Debug
- Information
- Warning
- Error
- Critical

Can be configured
- With Configuration Builder
- In Code

Also allows log level per namespace, allowing you to override log levels in a tree structure.  In the Example Below you'd get all queries and commands from Entity Framework but only Errors from the rest of Microsoft Components
        
        "Microsoft": "Error",
        "Microsoft.EntityFrameworkCore.Database": "Information"


## But Hosted Services have an extra layer
For hosted services there is a Host Builder it breaks up 

- Host Configuration
- App Configuration
- Logging
- Services
    - Adds a special type of registration for IHostedService
    
            await new HostBuilder().ConfigureHostConfiguration(configuration =>
            {
                //Get your enviornment variables with command line overrides
                configuration
                    .AddEnvironmentVariables("DOTNETCORE_")
                    .AddCommandLine(args);
            })
            .ConfigureAppConfiguration((hostContext, configuration) =>
            {
                //get your config files, per environment
                configuration
                    .AddJsonFile("appsettings.json", false, true)
                    .AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json",
                        false, true);
                
                //optionally add in azure app config by setting a connection string AppConfig
                var appConfig = configuration.Build().GetConnectionString("AppConfig");
                if(!string.IsNullOrWhiteSpace(appConfig))
                    configuration.AddAzureAppConfiguration(appConfig);
    
            })
            .ConfigureLogging((hostContext, logging) =>
            {
                //setup logging including azure monitor
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
                //register telemetry, your dependencies and your HostedService
                services
                    .AddApplicationInsightsTelemetryWorkerService()
                    .AddSingleton<IMyDependency1, MyDependency1>()
                    .AddHostedService<MyWoker>();
            })
            .RunConsoleAsync();
    
 Available out of the box IHostedServices
 - ASP.NETcore - Microsoft.AspNetCore
 - WebJobs - Microsoft.Azure.WebJobs.Core
 - Windows Services - Microsoft.Extensions.Hosting.WindowsServices
 
 ## Extras 
 - InMemory EF Core Database - not just for testing
 - Azure Application Insights/Monitor for tracking stats
 
 ## Demos
- Console Application for a TODO List using InMemory EF Core
    - Unit Tests 
        - Wrapping the Console
    - Running in a debugger
    - Running via command line
- Custom Worker Service using AppInsights to track random number metrics
    - Unit Tests 
        - Wrapping Microsoft's TelemetryClient
    - Running in a debugger
    - Running via command line
 
 
 