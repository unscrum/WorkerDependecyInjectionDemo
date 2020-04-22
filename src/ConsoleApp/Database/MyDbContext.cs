using Microsoft.EntityFrameworkCore;

namespace ConsoleApp.Database
{
    public class MyDbContext:DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
            
        }

        public DbSet<ToDo> ToDos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ToDo>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.Text).IsRequired();
                e.Property(p => p.Written).IsRequired();
            });

        }
    }
}