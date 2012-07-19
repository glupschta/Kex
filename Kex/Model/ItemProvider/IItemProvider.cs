using System.Collections.Generic;
using Kex.Model.Item;

namespace Kex.Model.ItemProvider
{
    public interface IItemProvider<T> 
        where T : IItem
    {
        IEnumerable<T> GetItems(string container);
    }
}
