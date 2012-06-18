using System;
using System.Collections.Generic;
using Kex.Controller;
using Kex.Model;
using Kex.Model.ItemProvider;

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

            var currentView = ListerManager.Instance.ListerViewManager.CurrentListerView;
            var listerType = currentView != null
                                       ? currentView.Lister.GetType()
                                       : typeof (FileLister);
            _locations[_currentIndex] = new HistoryItem(newLocation, _currentIndex, listerType);
        }

        public Dictionary<int, HistoryItem> Locations
        {
            get { return _locations; }
        }

    }

    public class HistoryItem
    {
        public HistoryItem(string fullpath, int index, Type listerType)
        {
            FullPath = fullpath;
            SelectedPath = null;
            Index = index;
            ListerType = listerType;
        }

        public string FullPath { get; set; }
        public string SelectedPath { get; set; }
        public int Index { get; set; }
        public Type ListerType { get; set; }
    }
}
