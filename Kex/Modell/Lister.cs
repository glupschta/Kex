using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Kex.Interfaces;

namespace Kex.Modell
{
    public class Lister : ILister
    {
        public Lister()
        {
            NavigationHistory = new BrowsingHistory();
        }

        public BrowsingHistory NavigationHistory { get; set; }

        private IItemProvider _itemProvider;
        public IItemProvider ItemProvider
        {
            get { return _itemProvider; }
            set { 
                _itemProvider = value;
                CurrentDirectory = _itemProvider.CurrentContainer;
            }
        }

        public string CurrentDirectory
        {
            get { return _currentDirectory; }
            set
            {
                NavigationHistory.Push(value, _currentDirectory);
                ItemProvider.CurrentContainer = value;
                _currentDirectory = value;
                if (value != null)
                {
                    _dirInfo = new DirectoryInfo(_currentDirectory);
                    
                }
                Refresh();
                if (PropertyChanged == null) return;
                OnPropertyChanged("CurrentDirectory");
                OnPropertyChanged("PathParts");
                OnPropertyChanged("SelectedItems");
            }
        }

        public string Filter
        {
            get { return _filter; }
            set { 
                _filter = value;
                OnPropertyChanged("Filter");
                OnPropertyChanged("Items");
            }
        }

        public string SortProperty
        {
            get { return _sortProperty; }
            set
            {
                _sortProperty = value;
                OnPropertyChanged("SortProperty");
                OnPropertyChanged("Items");
            }
        }

        public bool SortDescending { get; set; }

        private IEnumerable<IItem> _items;
        public IEnumerable<IItem> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
                OnPropertyChanged("Items");
            }
        }



        public ItemSelection SelectedItems
        {
            get
            {
                var selection = Items.Where(it => it.IsSelected);
                if (!selection.Any())
                    return null;
                return new ItemSelection
                           {
                               Selection = selection
                           };
            }
        }

        public void Refresh()
        {
            Filter = null;
            SortProperty = null;
            Items = ItemProvider.GetItems();
        }

        public string DirectoryUp()
        {
            return (_dirInfo.Parent != null) ? _dirInfo.Parent.FullName : null;
        }

        public string HistoryBack()
        {
            return NavigationHistory.Previous;
        }

        public string HistoryForward()
        {
            return NavigationHistory.Next;
        }
        
        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        private DirectoryInfo _dirInfo;
        private string _currentDirectory;
        private string _filter;
        private string _sortProperty;
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
