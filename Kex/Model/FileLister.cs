using System;
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
                NavigationHistory.Push(value, _currentDirectory, value);
                ItemProvider.CurrentContainer = value;
                _currentDirectory = value;
                Items = ItemProvider.GetItems();
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
            string parentDirectory = null;
            try
            {
                var directory = new DirectoryInfo(CurrentDirectory);
                if (CurrentDirectory.StartsWith(@"\\") && CurrentDirectory.Count(c => c == '\\') == 3)
                {
                    var index = CurrentDirectory.IndexOf("\\", 2);
                    return index != -1 ? CurrentDirectory.Substring(0, index) : null;
                }
                else
                {
                    parentDirectory = directory.Parent != null ? directory.Parent.FullName : null;
                }
            } catch (Exception ex)
            {
                
            }
            return parentDirectory;
        }

        public HistoryItem HistoryBack()
        {
            return NavigationHistory.Previous;
        }

        public HistoryItem HistoryForward()
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

        private string _currentDirectory;
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
