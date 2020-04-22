using Microsoft.EntityFrameworkCore;

namespace ConsoleApp.Test
{
    public static class InMemoryDbCreator
    {
        public static DbContextOptions<T> CreateInMemoryOptions<T>(string inMemoryDbBName) where T: DbContext {
            return new DbContextOptionsBuilder<T>()
                .UseInMemoryDatabase(inMemoryDbBName)
                .Options;
        }
    }
}