using System.Collections.Generic;

namespace Kex.Common
{
    public class BrowsingHistory
    {
        private readonly Dictionary<int, HistoryItem> _locations = new Dictionary<int, HistoryItem>(20);
        private int _currentIndex = -1;
        private int _maxIndex = -1;

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
                if (_currentIndex < _locations.Count-1 && _currentIndex<= _maxIndex)
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
            _maxIndex = _currentIndex;
            _locations[_currentIndex] = new HistoryItem(newLocation, _currentIndex);
        }

        public Dictionary<int, HistoryItem> Locations
        {
            get { return _locations; }
        }

    }

    public class HistoryItem
    {
        public HistoryItem(string fullpath, int index)
        {
            FullPath = fullpath;
            SelectedPath = null;
            Index = index;
        }

        public string FullPath { get; set; }
        public string SelectedPath { get; set; }
        public int Index { get; set; }
    }
}
