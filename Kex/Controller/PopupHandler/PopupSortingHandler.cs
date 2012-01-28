using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Kex.Common;

namespace Kex.Controller.PopupHandler
{
    public class PopupSortingHandler : IPopupHandler
    {
        public string Name
        {
            get { return "Sort"; }
        }

        public MatchMode MatchMode
        {
            get { return MatchMode.StartsWith; }
        }

        public IEnumerable<string> ListItems
        {
            get
            {
                var grid = ListerManager.Instance.ListerViewManager.CurrentListerView.View.View as GridView;
                if (grid != null)
                    return grid.Columns.Select(g => g.Header as string).Where(g => !string.IsNullOrEmpty(g)).ToArray();
                return new List<String> {};
            }
        }

        public void Close()
        {
        }

        public void ItemSelected(string item)
        {
            DoSort(item);
        }

        public void HandleKey(object sender, KeyEventArgs e)
        {
        }

        public void TextChanged(string text)
        {
        }

        private void DoSort(string property)
        {
            if (string.IsNullOrEmpty(property))
            {
                ListerManager.Instance.CommandManager.SetSorting(null);
                return;
            }
            if (property.Length == 0) return;
            var sortProperties = ListItems;
            var selectedColumn = sortProperties.Where(prop => prop.StartsWith(property, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            if (selectedColumn == null) return;
            ListerManager.Instance.CommandManager.SetSorting(selectedColumn);
        }

        public Func<string, string, bool> Filter
        {
            get { return null; }
        }
    }
}
