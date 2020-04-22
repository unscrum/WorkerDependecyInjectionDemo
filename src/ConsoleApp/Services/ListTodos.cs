using System;
using System.Linq;
using System.Threading.Tasks;
using ConsoleApp.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ConsoleApp.Services
{
    public interface IListTodos
    {
        Task ListAsync();
    }
    class ListTodos : IListTodos
    {
        private readonly MyDbContext _db;
        private readonly IMyConsole _console;
        private readonly ILogger _logger;
        
        public ListTodos(ILoggerFactory loggerFactory, MyDbContext db, IMyConsole console)
        {
            _logger = loggerFactory.CreateLogger<ListTodos>();
            _db = db;
            _console = console;
        }
        public async Task ListAsync()
        {
            _logger.LogInformation("Listing Todos");
            _console.Clear();
            var todos = await _db.ToDos.OrderByDescending(p => p.Written).ToArrayAsync();
            if (!todos.Any())
                _console.WriteLine("No TODOs in database.  Add one and try again.");
            foreach (var todo in todos)
                _console.WriteLine(todo.Text);
        }
    }
}