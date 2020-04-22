using System;
using System.Threading.Tasks;
using ConsoleApp.Database;
using Microsoft.Extensions.Logging;

namespace ConsoleApp.Services
{
    public interface IAddTodos
    {
        Task AwaitAddAsync();
    }
    class AddTodos: IAddTodos
    {
        private readonly MyDbContext _db;
        private readonly IMyConsole _console;
        private readonly ILogger _logger;

        public AddTodos(ILoggerFactory loggerFactory, MyDbContext db, IMyConsole console)
        {
            _logger = loggerFactory.CreateLogger<AddTodos>();
            _db = db;
            _console = console;
        }
        
        public async Task AwaitAddAsync()
        {
            try
            {
                _logger.LogInformation("Adding a new Todo.");
                _console.Clear();
                _console.WriteLine("Enter a todo item, then press enter:");
                var response = _console.ReadLine();
                await _db.ToDos.AddAsync(new ToDo
                {
                    Id = Guid.NewGuid(),
                    Written = DateTime.UtcNow,
                    Text = response
                });
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(AwaitAddAsync)} encountered an error");
            }
        }
    }
}