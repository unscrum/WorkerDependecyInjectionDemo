using System;

namespace ConsoleApp.Services
{
    public interface IMyConsole
    {
        void Clear();
        void WriteLine(string line);
        void WriteEmptyLine();
        string ReadLine();
    }
    public class MyConsole:IMyConsole
    {
        public void Clear()
        {
           Console.Clear();
        }

        public void WriteLine(string line)
        {
           Console.WriteLine(line);
        }

        public void WriteEmptyLine()
        {
            Console.WriteLine();
        }

        public string ReadLine()
        {
            return Console.ReadLine();
        }
    }
}