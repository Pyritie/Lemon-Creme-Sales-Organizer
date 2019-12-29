using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public interface ITransaction
    {
        IEnumerable<IItem> Items { get; }
    }
}
