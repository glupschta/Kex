using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using Kex.Common;
using Kex.Model.ItemProvider;
using Kex.Model;

namespace Kex.Model
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

        public void SelectionChanged(ListView view, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            Selection = view.SelectedItems.OfType<FileItem>().ToList();
            SelectionCount = view.SelectedItems.Count;
            SelectionSize = view.SelectedItems.OfType<FileItem>().Sum(i => i.Length);
            MainWindow.Debug("Selection Count "+ Selection.Count());
        }

        public string CurrentDirectory
        {
            get { return _currentDirectory; }
            set
            {
                _currentDirectory = value;
                ItemProvider.CurrentContainer = value;
                Refresh();
                NavigationHistory.Push(value);
            }
        }

        public string Filter
        {
            get { return _filter; }
            set { 
                _filter = value;
                OnPropertyChanged("Filter");
            }
        }

        public int SelectionCount
        {
            get { return _selectionCount; }
            set
            {
                _selectionCount = value;
                OnPropertyChanged("SelectionCount");
            }
        }

        public long SelectionSize
        {
            get { return _selectionSize; }
            set 
            { 
                _selectionSize = value;
                OnPropertyChanged("SelectionSize");
            }
        }

        public IEnumerable<IItem> Selection { get; set; }

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

        public void Refresh()
        {
            Items = ItemProvider.GetItems();
        }

        public string ContainerUp()
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
        private string _filter;
        private int _selectionCount;
        private long _selectionSize;
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
