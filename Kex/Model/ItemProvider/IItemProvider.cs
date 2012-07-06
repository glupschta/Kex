using System.Collections.Generic;

namespace Kex.Model.ItemProvider
{
    public interface IItemProvider<T> 
        where T : IItem
    {
        IEnumerable<T> GetItems(string container);
    }
}
