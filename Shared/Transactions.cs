﻿using System.Collections.Generic;

namespace Shared
{
    public interface ITransaction
    {
        IEnumerable<IItem> Items { get; }
    }
}
