using System.Collections.Generic;

namespace SalesOrganizer.Models
{
    public interface ITransaction
    {
        IEnumerable<IItem> Items { get; }
    }

    public class Sale : ITransaction
    {
        public IEnumerable<IItem> Items { get; }
    }
}
