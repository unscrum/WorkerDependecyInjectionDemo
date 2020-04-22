using System;

namespace ConsoleApp.Services
{
    interface IMyConsole
    {
        void Clear();
        void WriteLine(string line);
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

        public string ReadLine()
        {
            return Console.ReadLine();
        }
    }
}