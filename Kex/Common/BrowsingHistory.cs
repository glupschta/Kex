using System.Collections.Generic;

namespace Kex.Common
{
    public class BrowsingHistory
    {
        private readonly List<string> _locations = new List<string>();
        private int _currentIndex = -1;

        public string Current
        {
            get
            {
                return _locations[_currentIndex];
            }
        }

        public string Previous
        {
            get
            {
                if (_currentIndex > 0)
                    _currentIndex--;
                return Current;
            }
        }

        public string Next
        {
            get 
            { 
                if (_currentIndex < _locations.Count-1)
                    _currentIndex++;
                return Current;
            }
        }

        public void Push(string newLocation, string oldLocation)
        {
            if (_currentIndex > -1 && newLocation == _locations[_currentIndex]) return;
            _currentIndex++;
            _locations.Add(newLocation);
        }

        public List<string> Locations
        {
            get { return _locations; }
        }

    }
}
