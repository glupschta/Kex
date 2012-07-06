using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using Kex.Common;
using Kex.Model.ItemProvider;

namespace Kex.Model.Lister
{
    public interface ILister : INotifyPropertyChanged, IColumnProvider
    {
        BrowsingHistory NavigationHistory { get; set; }
        IEnumerable<IItem> Items { get; set; }
        void Refresh();

        string CurrentDirectory { get; set; }
        string Filter { get; set; }
        int SelectionCount { get; set; }
        long SelectionSize { get; set; }
        IEnumerable<IItem> Selection { get; set; }
        ListView ListView { get; set; }
        string XamlView { get; }

        HistoryItem HistoryBack();
        HistoryItem HistoryForward();
        string ContainerUp();

        void OnPropertyChanged(string property);
        void DoAction(object item);
        void SelectionChanged(ListView view, SelectionChangedEventArgs selectionChangedEventArgs);
    }

    public interface ILister<T> : ILister
        where T : IItem
    {
        IItemProvider<T> ItemProvider { get; set; }
    }
}
