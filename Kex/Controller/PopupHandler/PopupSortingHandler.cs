﻿using System;
using System.Collections.Generic;
using System.Linq;
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
            get { return new List<String> { "Name", "Created", "LastModified", "Length" }; }
        }

        public void Close()
        {
        }

        public void ItemSelected(string item)
        {
            DoSort(item, true);
        }

        public void HandleKey(object sender, KeyEventArgs e)
        {
        }

        public void TextChanged(string text)
        {
        }

        private void DoSort(string property, bool descending)
        {
            if (string.IsNullOrEmpty(property))
            {
                MessageHost.ViewHandler.SetSorting(null, descending);
                return;
            }
            if (property.Length == 0) return;
            var sortProperties = ListItems;
            var selectedColumn = sortProperties.Where(prop => prop.StartsWith(property, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            if (selectedColumn == null) return;
            MessageHost.ViewHandler.SetSorting(selectedColumn, descending);
        }
    }
}
