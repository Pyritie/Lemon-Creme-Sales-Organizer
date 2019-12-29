using System.Collections.Generic;

namespace Shared
{
    public interface IItem
    {
        string Name { get; }
        string Type { get; }
        decimal Worth { get; }
    }

    public class InventoryItem : IItem
    {
        public string Name { get; }
        public string Type { get; }
        public decimal Worth { get; }

        public Dictionary<Person, decimal> Owners { get; }


        public InventoryItem(string name, string type, decimal worth)
        {
            Name = name;
            Type = type;
            Worth = worth;

            Owners = new Dictionary<Person, decimal>();
        }
    }

    public class TaxItem : IItem
    {
        public string Name => "Tax";
        public string Type => "Tax";
        public decimal Worth { get; }

        public TaxItem(decimal worth)
        {
            Worth = worth;
        }
    }
}
