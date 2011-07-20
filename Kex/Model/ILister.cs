using System.Collections.Generic;
using System.ComponentModel;
using Kex.Common;
using Kex.Model.ItemProvider;
using Kex.Modell;

namespace Kex.Model
{
    public interface ILister : INotifyPropertyChanged
    {
        BrowsingHistory NavigationHistory { get; set; }
        IEnumerable<IItem> Items { get; set; }
        void Refresh();

        ItemSelection SelectedItems { get; }
        string CurrentDirectory { get; set; }

        string HistoryBack();
        string HistoryForward();
        string DirectoryUp();

        void OnPropertyChanged(string property);
    }

    public interface ILister<T> : ILister
    {
        IItemProvider<T> ItemProvider { get; set; }
    }
}
