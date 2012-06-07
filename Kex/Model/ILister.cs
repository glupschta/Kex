using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using Kex.Common;
using Kex.Model.ItemProvider;
using Kex.Model;

namespace Kex.Model
{
    public interface ILister : INotifyPropertyChanged
    {
        BrowsingHistory NavigationHistory { get; set; }
        IEnumerable<IItem> Items { get; set; }
        void Refresh();

        string CurrentDirectory { get; set; }
        string Filter { get; set; }
        int SelectionCount { get; set; }
        long SelectionSize { get; set; }

        HistoryItem HistoryBack();
        HistoryItem HistoryForward();
        string ContainerUp();

        void OnPropertyChanged(string property);
    }

    public interface ILister<T> : ILister
    {
        IItemProvider<T> ItemProvider { get; set; }
        void SelectionChanged(ListView view, SelectionChangedEventArgs selectionChangedEventArgs);
    }
}
