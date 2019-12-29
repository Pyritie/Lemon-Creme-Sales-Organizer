using System.Collections.Generic;

namespace SalesOrganizer.Models
{
    public interface ITransaction
    {
        IEnumerable<IItem> Items { get; }
    }
}
