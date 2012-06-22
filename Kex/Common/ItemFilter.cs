using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kex.Common
{
    public class ItemFilter : IEnumerable<IPopupItem>
    {
        private IEnumerable<IPopupItem> _items;
        private readonly string _filterString;
        private IEnumerable<FilterPart> _filterParts;

        public ItemFilter(IEnumerable<IPopupItem> items, string filterString)
        {
            _items = items;
            _filterString = filterString.Trim();
            if (!string.IsNullOrEmpty(filterString))
            {
                _filterParts = _filterString
                    .Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => new FilterPart(p));

            }
            if (_filterParts == null || !FilterParts.Any()) _filterParts = new List<FilterPart> { new FilterPart(_filterString) };
        }

        public IEnumerable<FilterPart> FilterParts
        {
            get { return _filterParts.Where(f => f.HasFilter); }
        }

        public IEnumerable<IPopupItem> MatchesBeginning
        {
            get
            {
                return _items.Where(i => matchItemStartsWith(i, FilterParts.FirstOrDefault()));
            }
        }

        public IEnumerable<IPopupItem> MatchesContaining
        {
            get
            {
                return FilterParts
                    .Aggregate(_items, (current, part) => current.Where(i => matchItemContaining(i, part))
                    );
            }            
        }

        private bool matchItemStartsWith(IPopupItem item, FilterPart part)
        {
            if (!FilterParts.Any()) return true;
            var ret = item.FilterString.StartsWith(FilterParts.First().FilterString, StringComparison.OrdinalIgnoreCase);
            return (part.Negate) ? !ret: ret;
        }

        private bool matchItemContaining(IPopupItem item, FilterPart part)
        {
            if (part.Negate)
                return item.FilterString.IndexOf(part.FilterString, StringComparison.OrdinalIgnoreCase) == -1;
            return item.FilterString.IndexOf(part.FilterString, StringComparison.OrdinalIgnoreCase) != -1;
        }

        public IEnumerable<IPopupItem> Matches
        {
            get
            {
                return FilterParts.Count() == 1 && !FilterParts.First().Negate 
                    ? MatchesBeginning.Union(MatchesContaining, new PopupItemComparer()) 
                    : MatchesContaining;
            }
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

    public class FilterPart
    {
        public FilterPart(string part)
        {
           if (part.Any() && part[0] == '-')
           {
               if (part.Length > 1 && part[1] == '-')
               {
                   Negate = false;
               }
               else
               {
                   Negate = true;
                   
               }
               FilterString = part.Substring(1);
           } 
           else
           {
               Negate = false;
               FilterString = part;
           }
        }

        public string FilterString { get; set; }
        public bool Negate { get; set; }

        public bool HasFilter
        {
            get { return !string.IsNullOrEmpty(FilterString); }
        }
    }
}
