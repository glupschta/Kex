using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Kex.Interfaces
{
    public interface IItemProvider
    {
        string CurrentContainer { get; set; }
        IEnumerable<IItem> GetItems();
        void DoAction(IItem item);
    }
}
