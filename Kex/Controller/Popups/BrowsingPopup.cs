using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Kex.Common;
using Kex.Model;
using Kex.Model.Item;
using Kex.Views;

namespace Kex.Controller.Popups
{
    public class BrowsingPopup : BasePopup
    {
        public BrowsingPopup(IPopupInput input): base(input)
        {
            ListItems = ListerManager.Instance.CommandManager.CurrentView.View.Items.Cast<IPopupItem>();
        }

        public override string Name { get { return "Browse"; } }

        protected override void HandleSelection()
        {
            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                ListerManager.Instance.ListerViewManager.OpenLister(ListerManager.Instance.CommandManager.CurrentItem.FullPath);
                Hide();
            }
            else
            {
                ListerManager.Instance.CommandManager.DoDefaultAction();
            }
            //no Textchanged if Text="" after Clear
            if (string.IsNullOrEmpty(Text))
            {
                updateItems();
            }
            else
            {
                Input.TextBox.Clear();
            }
            Keyboard.Focus(Input.TextBox);
        }

        protected override void ListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            SetGridSelection(GetSelectedListItem<IPopupItem>());
        }

        protected override void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var itemsFound = updateItems();
            if (!itemsFound && Text.Any() && !string.IsNullOrEmpty(Text) && Text.Last() != '-')
            {
                Text = Text.Substring(0, Text.Length - 1);
                Input.TextBox.CaretIndex = Text.Length;
            }
        }
                                    
        private bool updateItems()
        {
            var items = ListerManager.Instance.CommandManager.CurrentView.View.Items.Cast<IPopupItem>();
            var filtered = new ItemFilter(items, Text);
            if (!filtered.Any())
                return false;
            SetGridSelection(filtered.MatchesBeginning.FirstOrDefault() ?? filtered.MatchesContaining.FirstOrDefault());
            ListItems = filtered;
            Input.ListBox.SelectedIndex = 0;
            return true;
        }

        private void SetGridSelection(IPopupItem selection)
        {
            var item = selection as IItem;
            if (item != null)
            {
                ListerManager.Instance.CommandManager.CurrentItem = item;
                Keyboard.Focus(Input.TextBox);
            }
        }

    }
}
