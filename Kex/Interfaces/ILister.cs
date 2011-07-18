using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Kex.Common;
using Kex.Modell;

namespace Kex.Interfaces
{
    public interface ILister : INotifyPropertyChanged
    {
        BrowsingHistory NavigationHistory { get; set; }
        IItemProvider ItemProvider { get; set; }
        IEnumerable<IItem> Items { get; set; }
        void Refresh();

        ItemSelection SelectedItems { get; }
        string CurrentDirectory { get; set; }
        string Filter { get; set; }
        string SortProperty { get; set; }
        bool SortDescending { get; set; }

        string HistoryBack();
        string HistoryForward();
        string DirectoryUp();

        void OnPropertyChanged(string property);
    }
}
