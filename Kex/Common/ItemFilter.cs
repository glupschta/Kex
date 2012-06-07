using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kex.Common
{
    public class ItemFilter : IEnumerable<IPopupItem>
    {
        private readonly IEnumerable<IPopupItem> _items;
        private readonly string _filterString;

        public ItemFilter(IEnumerable<IPopupItem> items, string filterString)
        {
            _items = items;
            _filterString = filterString;
        }

        public IEnumerable<IPopupItem> MatchesBeginning
        {
            get { return _items.Where(i => i.FilterString.StartsWith(_filterString, StringComparison.OrdinalIgnoreCase)); }
        }

        public IEnumerable<IPopupItem> MatchesContaining
        {
            get { return _items.Where(i => i.FilterString.IndexOf(_filterString, StringComparison.OrdinalIgnoreCase) > -1); }            
        }

        public IEnumerable<IPopupItem> Matches
        {
            get { return MatchesBeginning.Union(MatchesContaining, new PopupItemComparer()); }
        }

        public IEnumerator<IPopupItem> GetEnumerator()
        {
            return Matches.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        class PopupItemComparer : IEqualityComparer<IPopupItem>
        {
            public bool Equals(IPopupItem x, IPopupItem y)
            {
                return x.FilterString == y.FilterString;
            }

            public int GetHashCode(IPopupItem obj)
            {
                if (ReferenceEquals(obj, null) || obj.FilterString ==null) return 0;
                return obj.FilterString.GetHashCode();
            }
        }
    }
}
