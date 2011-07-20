using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Kex.Common;
using Kex.Model;
using Kex.Model.ItemProvider;

namespace Kex.Modell
{
    public class FileLister : ILister<FileProperties>
    {
        public FileLister()
        {
            NavigationHistory = new BrowsingHistory();
        }

        public BrowsingHistory NavigationHistory { get; set; }

        private IItemProvider<FileProperties> _itemProvider;
        public IItemProvider<FileProperties> ItemProvider
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
                Items = ItemProvider.GetItems();
                OnPropertyChanged("Items");
                //Refresh();
                //if (PropertyChanged == null) return;
                //OnPropertyChanged("CurrentDirectory");
                //OnPropertyChanged("PathParts");
                //OnPropertyChanged("SelectedItems");
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
