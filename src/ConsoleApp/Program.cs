using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ConsoleApp.Database;
using ConsoleApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly:InternalsVisibleTo("ConsoleApp.Test")]
namespace ConsoleApp
{
    public interface IProgram
    {
        public Task RunAsync();
    }
    class Program: IProgram
    {
        private readonly IMenu _menu;
        private readonly IListTodos _listTodos;
        private readonly IAddTodos _addTodos;
        private readonly IMyConsole _console;
        private readonly ILogger _logger;
        public Program(ILoggerFactory lf, IMenu menu, IListTodos listTodos, IAddTodos addTodos, IMyConsole console)
        {
            _logger = lf.CreateLogger<Program>();
            _menu = menu;
            _listTodos = listTodos;
            _addTodos = addTodos;
            _console = console;
        }

        static async Task Main(string[] args)
        {
            //create an initial configuration from environment variables
            var configurationBuilder = new ConfigurationBuilder().AddEnvironmentVariables("DOTNETCORE_")
                .AddCommandLine(args);
            //get the environment settings
            var environment = configurationBuilder.Build().GetValue<string>("Environment");

            //add config files per environment
            configurationBuilder.AddJsonFile("consolesettings.json", false, false);
            configurationBuilder.AddJsonFile($"consolesettings.{environment}.json", false, false);
            //build the configuration for the container
            var configuration = configurationBuilder.Build();

            //create and build the container
            var services = new ServiceCollection()
                .AddSingleton<IConfiguration>(configuration)
                .AddDbContext<MyDbContext>(o => o.UseInMemoryDatabase("ConsoleApp"))
                .AddScoped<IMenu, Menu>()
                .AddScoped<IListTodos, ListTodos>()
                .AddScoped<IAddTodos, AddTodos>()
                .AddSingleton<IMyConsole, MyConsole>()
                .AddScoped<IProgram, Program>()
                .AddLogging(lb =>
                {
                    lb.AddConfiguration(configuration)
                        .AddDebug()
                        .AddConsole();
                })
               .BuildServiceProvider();

            //create a scope for the application to run in
            using (var scope = services.CreateScope())
            {
                //get the program out and run it.
                var program = scope.ServiceProvider.GetRequiredService<IProgram>();
                await program.RunAsync();
            }

        }

        public async Task RunAsync()
        {
            _logger.LogInformation("Starting up Program.");
            _console.Clear();
            _console.WriteLine("Welcome to the TODO List"); 
            MenuChoices choice;
            do
            {
                _menu.PrintMenu();
                choice = _menu.AwaitValidInput();
                switch (choice)
                {
                    case MenuChoices.ListTodos:
                        await _listTodos.ListAsync();
                        break;
                    case MenuChoices.AddTodo:
                        await _addTodos.AwaitAddAsync();
                        break;
                }
            } while (choice != MenuChoices.Exit);
        }
    }
}