using System.Collections.Generic;

namespace SalesOrganizer.Models
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

        public Dictionary<Person, decimal> Earners { get; }

        private ByName m_byName;
        public ByName EarnersByName => m_byName ?? (m_byName = new ByName(this));


        public InventoryItem(string name, string type, decimal worth)
        {
            Name = name;
            Type = type;
            Worth = worth;

            Earners = new Dictionary<Person, decimal>();
        }

        // this is so we can bind to the earners dict
        public sealed class ByName
        {
            private readonly InventoryItem m_item;

            public ByName(InventoryItem item)
            {
                m_item = item;
            }

            public decimal this[string name]
            {
                get
                {
                    foreach (var kvp in m_item.Earners)
                    {
                        if (kvp.Key.Name == name)
                            return kvp.Value;
                    }
                    return -1;
                }
            }
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
