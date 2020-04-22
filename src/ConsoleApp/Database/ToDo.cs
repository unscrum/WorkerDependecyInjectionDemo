using System;

namespace ConsoleApp.Database
{
    public class ToDo
    {
        public Guid Id { get; set; }
        public DateTime Written { get; set; }
        public string Text { get; set; }
    }
}