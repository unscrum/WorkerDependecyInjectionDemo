using System;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace ConsoleApp.Services
{
    public enum MenuChoices
    {
        ListTodos = 0,
        AddTodo = 1,
        Exit = 3
    }
    public interface IMenu
    {
        void PrintMenu();
        MenuChoices AwaitValidInput();
    }
    class Menu:IMenu
    {
        private readonly ILogger _logger;

        public Menu(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Menu>();
        }

        public void PrintMenu()
        {
            Console.WriteLine();
            Console.WriteLine("Please make a selection and press enter.");
            PrintChoices();
        }

        private void PrintChoices()
        {
            Console.WriteLine("\t1) List Todos");
            Console.WriteLine("\t2) Add Todo");
            Console.WriteLine("\t3) Exit");
        }

        public MenuChoices AwaitValidInput()
        {
            try
            {
                var input = Console.ReadLine();
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
            Console.Clear();
            Console.WriteLine("Please make a valid selection");
            PrintChoices();
        }
    }
}