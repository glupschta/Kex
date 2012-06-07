using System.Collections.Generic;

namespace Kex.Model.ItemProvider
{
    public interface IItemProvider<T> : IColumnProvider
    {
        string CurrentContainer { get; set; }
        void DoAction(IItem item);
        IEnumerable<IItem<T>> GetItems();
        T FetchDetails(IItem<T> item);
    }
}
