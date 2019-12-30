using System.Diagnostics;

namespace SalesOrganizer.Models
{
    [DebuggerDisplay("Person({Name})")]
    public class Person
    {
        public string Name { get; }

        public Person(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
