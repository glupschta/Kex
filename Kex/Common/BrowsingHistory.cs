using System.Collections.Generic;

namespace Kex.Common
{
    public class BrowsingHistory
    {
        private readonly List<HistoryItem> _locations = new List<HistoryItem>();
        private int _currentIndex = -1;

        public HistoryItem Current
        {
            get
            {
                return _locations[_currentIndex];
            }
        }

        public HistoryItem Previous
        {
            get
            {
                if (_currentIndex > 0)
                    _currentIndex--;
                return Current;
            }
        }

        public HistoryItem Next
        {
            get 
            { 
                if (_currentIndex < _locations.Count-1)
                    _currentIndex++;
                return Current;
            }
        }

        public void Push(string newLocation, string oldLocation, string selectedItem)
        {
            if (_currentIndex > -1 && newLocation == _locations[_currentIndex].FullPath) return;
            _currentIndex++;
            _locations.Add(new HistoryItem(newLocation, selectedItem));
        }

        public List<HistoryItem> Locations
        {
            get { return _locations; }
        }

    }

    public class HistoryItem
    {
        public HistoryItem(string fullpath, string selectedPath)
        {
            FullPath = fullpath;
            SelectedPath = selectedPath;
        }

        public string FullPath { get; set; }
        public string SelectedPath { get; set; }
    }
}
