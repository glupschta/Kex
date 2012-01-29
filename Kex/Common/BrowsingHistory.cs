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
                return _currentIndex < 0 ? null : _locations[_currentIndex];
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

        public void Push(string newLocation)
        {
            if (_currentIndex > -1 && newLocation == _locations[_currentIndex].FullPath) 
                return;
            if (Current != null) 
                Current.SelectedPath = newLocation;

            _currentIndex++;
            _locations.Add(new HistoryItem(newLocation));
        }

        public List<HistoryItem> Locations
        {
            get { return _locations; }
        }

    }

    public class HistoryItem
    {
        public HistoryItem(string fullpath)
        {
            FullPath = fullpath;
            SelectedPath = null;
        }

        public string FullPath { get; set; }
        public string SelectedPath { get; set; }
    }
}
