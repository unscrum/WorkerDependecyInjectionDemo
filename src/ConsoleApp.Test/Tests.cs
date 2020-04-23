using System;
using System.Linq;
using System.Threading.Tasks;
using ConsoleApp.Database;
using ConsoleApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace ConsoleApp.Test
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public async Task Test_List()
        {
            var console = new Mock<IMyConsole>(MockBehavior.Loose);
            var ctx = new MyDbContext(InMemoryDbCreator.CreateInMemoryOptions<MyDbContext>("Test_List"));
            await ctx.ToDos.AddRangeAsync(new ToDo()
            {
                Id = Guid.NewGuid(),
                Text = "Existing1",
                Written = DateTime.UtcNow
            }, new ToDo()
            {
                Id = Guid.NewGuid(),
                Text = "Existing2",
                Written = DateTime.UtcNow

            });
            await ctx.SaveChangesAsync();
            var list = new ListTodos(NullLoggerFactory.Instance, ctx, console.Object);
            console.Setup(p => p.WriteLine(It.IsAny<string>()));
            await list.ListAsync();
            console.Verify(p => p.WriteLine("Existing1"), Times.Once);
            console.Verify(p => p.WriteLine("Existing2"), Times.Once);

        }
        [Test]
        public async Task Test_EmptyList()
        {
            var console = new Mock<IMyConsole>(MockBehavior.Loose);
            var ctx = new MyDbContext(InMemoryDbCreator.CreateInMemoryOptions<MyDbContext>("Test_EmptyList"));
            var list = new ListTodos(NullLoggerFactory.Instance, ctx, console.Object);
            console.Setup(p => p.WriteLine(It.IsAny<string>()));
            await list.ListAsync();
            console.Verify(p => p.WriteLine("No TODOs in database.  Add one and try again."), Times.Once);

        }
        
        [Test]
        public async Task Test_Add()
        {
            var console = new Mock<IMyConsole>(MockBehavior.Loose);
            var ctx = new MyDbContext(InMemoryDbCreator.CreateInMemoryOptions<MyDbContext>("Test_Add"));
            var add = new AddTodos(NullLoggerFactory.Instance, ctx, console.Object);
            console.Setup(p => p.WriteLine(It.IsAny<string>()));
            console.Setup(p => p.ReadLine()).Returns("New TODO");
            await add.AwaitAddAsync();
            
            console.Verify(p => p.WriteLine("Enter a todo item, then press enter:"), Times.Once);
            console.Verify(p => p.ReadLine(), Times.Once);
            Assert.AreEqual(1,await ctx.ToDos.CountAsync());
            Assert.AreEqual("New TODO",await ctx.ToDos.Select(p=>p.Text).FirstAsync());

        }
        
        [Test]
        public void Test_Menu()
        {
            var console = new Mock<IMyConsole>(MockBehavior.Loose);
            var menu = new Menu(NullLoggerFactory.Instance, console.Object);
            console.Setup(p => p.WriteLine(It.IsAny<string>()));
            console.Setup(p => p.ReadLine()).Returns("New TODO");
            menu.PrintMenu();
            
            console.Verify(p => p.WriteEmptyLine(), Times.Once);
            console.Verify(p => p.WriteLine(It.IsAny<string>()), Times.Exactly(4));
            console.Verify(p => p.ReadLine(), Times.Never);

        }
        
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void Test_Menu_Read(int value)
        {
            var console = new Mock<IMyConsole>(MockBehavior.Loose);
            var menu = new Menu(NullLoggerFactory.Instance, console.Object);
            console.Setup(p => p.ReadLine()).Returns(value.ToString);
            
            Assert.AreEqual((MenuChoices)value, menu.AwaitValidInput());
        }
        [Test]
        public async Task Test_Program_Exit()
        {
            var console = new Mock<IMyConsole>(MockBehavior.Loose);
            var list = new Mock<IListTodos>(MockBehavior.Strict);
            var add = new Mock<IAddTodos>(MockBehavior.Strict);
            var menu = new Mock<IMenu>(MockBehavior.Loose);
            menu.Setup(p => p.AwaitValidInput()).Returns(MenuChoices.Exit);
            var program = new Program(NullLoggerFactory.Instance, menu.Object, list.Object, add.Object, console.Object);
            await program.RunAsync();
            
        }
        [Test]
        public async Task Test_Program_Add()
        {
            var console = new Mock<IMyConsole>(MockBehavior.Loose);
            var list = new Mock<IListTodos>(MockBehavior.Strict);
            var add = new Mock<IAddTodos>(MockBehavior.Strict);
            var menu = new Mock<IMenu>(MockBehavior.Loose);
            menu.Setup(p => p.AwaitValidInput()).Returns(MenuChoices.AddTodo);
            add.Setup(p => p.AwaitAddAsync()).Returns(Task.CompletedTask).Callback(() =>
            {
                menu.Setup(p => p.AwaitValidInput()).Returns(MenuChoices.Exit);
            });
            var program = new Program(NullLoggerFactory.Instance, menu.Object, list.Object, add.Object, console.Object);
            await program.RunAsync();
            
        }
        
        [Test]
        public async Task Test_Program_List()
        {
            var console = new Mock<IMyConsole>(MockBehavior.Loose);
            var list = new Mock<IListTodos>(MockBehavior.Strict);
            var add = new Mock<IAddTodos>(MockBehavior.Strict);
            var menu = new Mock<IMenu>(MockBehavior.Loose);
            menu.Setup(p => p.AwaitValidInput()).Returns(MenuChoices.ListTodos);
            list.Setup(p => p.ListAsync()).Returns(Task.CompletedTask).Callback(() =>
            {
                menu.Setup(p => p.AwaitValidInput()).Returns(MenuChoices.Exit);
            });
            var program = new Program(NullLoggerFactory.Instance, menu.Object, list.Object, add.Object, console.Object);
            await program.RunAsync();
            
        }
    }
}