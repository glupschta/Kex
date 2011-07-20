using System.Collections.Generic;

namespace Kex.Model.ItemProvider
{
    public interface IItemProvider<out T>
    {
        string CurrentContainer { get; set; }
        IEnumerable<IItem> GetItems();
        void DoAction(IItem item);
        T Fetch(IItem item);
    }
}
