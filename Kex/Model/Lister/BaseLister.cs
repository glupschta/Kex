using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Kex.Common;
using Kex.Controller;
using Kex.Model.Item;

namespace Kex.Model.Lister
{
    public abstract class BaseLister : ILister
    {
        protected BaseLister()
        {
            NavigationHistory = new BrowsingHistory();
        }

        public abstract string XamlView { get;  }
        public ListView ListView { get; set; }


        protected BaseLister(ILister lister)
        {
            NavigationHistory = lister.NavigationHistory;
        }

        protected BaseLister(ILister lister, string directory)
        {
            NavigationHistory = lister.NavigationHistory;
            CurrentDirectory = directory;
        }

        public BrowsingHistory NavigationHistory { get; set; }

        public virtual void SelectionChanged(ListView view, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            Selection = view.SelectedItems.OfType<FileItem>().ToList();
            SelectionCount = view.SelectedItems.Count;
            SelectionSize = view.SelectedItems.Cast<FileItem>().Sum(i => i.Properties.Length);
        }

        public virtual string CurrentDirectory
        {
            get { return _currentDirectory; }
            set
            {
                _currentDirectory = value;
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

        public abstract void Refresh();

        public string ContainerUp()
        {
            try
            {
                var directory = new DirectoryInfo(CurrentDirectory);
                if (CurrentDirectory.StartsWith(@"\\") && CurrentDirectory.Count(c => c == '\\') == 3)
                {
                    var index = CurrentDirectory.IndexOf("\\", 2);
                    return index != -1 ? CurrentDirectory.Substring(0, index) : null;
                }
                return directory.Parent != null ? directory.Parent.FullName : null;
            } catch (Exception ex)
            {
                MainWindow.Debug(ex);
                return null;
            }
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

        public abstract void DoAction(object obj);

        protected string _currentDirectory;
        private string _filter;
        private int _selectionCount;
        private long _selectionSize;
        public event PropertyChangedEventHandler PropertyChanged;

        public abstract IEnumerable<Column> Columns { get; }
    }
}
