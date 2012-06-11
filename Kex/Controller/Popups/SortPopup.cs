using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Kex.Common;
using Kex.Views;

namespace Kex.Controller.Popups
{
    public class SortPopup : BasePopup
    {
        public SortPopup(IPopupInput input) : base(input)
        {
            _allColumns = ListerManager.Instance.ListerViewManager.CurrentListerView.Lister.ItemProvider.Columns;   
            _allKeys = _allColumns.Select(s => new StringPopupItem(s.Key));
            ListItems = _allKeys;
        }

        public override void Show()
        {
            base.Show();
            var sortColumn = ListerManager.Instance.ListerViewManager.CurrentListerView.View.Items.SortDescriptions.FirstOrDefault();
            Text = _allColumns.Keys.Where(k => _allColumns[k] == sortColumn.PropertyName).FirstOrDefault();
            Input.TextBox.SelectAll();
        }

        protected override void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ListItems = new ItemFilter(_allKeys, Text);
            Input.ListBox.SelectedIndex = 0;
        }

        protected override void HandleSelection()
        {
            var selectedColumnKey = Input.ListBox.SelectedItem as StringPopupItem;
            if (selectedColumnKey != null)
            {
                var selectedColumn = _allColumns[selectedColumnKey.DisplayName];
                ListerManager.Instance.CommandManager.SetSorting(selectedColumn);
                Hide();
            }
        }

        public override string Name
        {
            get { return "Sorting"; }
        }

        private readonly Dictionary<string, string> _allColumns;
        private readonly IEnumerable<StringPopupItem> _allKeys;
    }
}
