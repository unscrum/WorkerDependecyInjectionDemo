using System;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace ConsoleApp.Services
{
    public interface IMenu
    {
        void PrintMenu();
        MenuChoices AwaitValidInput();
    }
    class Menu:IMenu
    {
        private readonly IMyConsole _console;
        private readonly ILogger _logger;

        public Menu(ILoggerFactory loggerFactory, IMyConsole console)
        {
            _console = console;
            _logger = loggerFactory.CreateLogger<Menu>();
        }

        public void PrintMenu()
        {
            _console.WriteEmptyLine();
            _console.WriteLine("Please make a selection and press enter.");
            PrintChoices();
        }

        private void PrintChoices()
        {
            _console.WriteLine("\t1) List Todos");
            _console.WriteLine("\t2) Add Todo");
            _console.WriteLine("\t3) Exit");
        }

        public MenuChoices AwaitValidInput()
        {
            try
            {
                var input = _console.ReadLine();
                if (string.IsNullOrWhiteSpace(input) || input.Length < 1)
                {

                    Error();
                    return AwaitValidInput();
                }

                var first = input.Trim().First();
                switch (first)
                {
                    case '1':
                        return MenuChoices.ListTodos;
                    case '2':
                        return MenuChoices.AddTodo;
                    case '3':
                        return MenuChoices.Exit;
                    default:
                        Error();
                        return AwaitValidInput();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,$"{nameof(AwaitValidInput)} encountered an error." );
                throw;
            }
        }

        private void Error()
        {
            _console.Clear();
            _console.WriteLine("Please make a valid selection");
            PrintChoices();
        }
    }
}