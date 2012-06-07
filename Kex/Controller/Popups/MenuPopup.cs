using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Kex.Common;
using Kex.Views;
using MenuItem = Kex.Common.MenuItem;

namespace Kex.Controller.Popups
{
    public class MenuPopup : BasePopup
    {
        public MenuPopup(IPopupInput input, string menuName) : base(input)
        {
            this.menuName = menuName;
            menuItems = MenuHandler.Instance.GetMenuByName(menuName).SubItems;
            ListItems = menuItems;
        }

        protected override void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ListItems = new ItemFilter(menuItems, Text);
            Input.ListBox.SelectedIndex = 0;
        }

        protected override void HandleSelection()
        {
            var selection = Input.ListBox.SelectedItem as MenuItem;
            if (selection == null) return;

            if (selection.SubItems.Any())
            {
                menuItems = selection.SubItems;
                Input.TextBox.Clear();
            }
            else
            {
                ListerManager.Instance.CommandManager.ExecuteByCommandName(selection.Command, selection.Argument);
                Hide();
            }
        }

        public override string Name
        {
            get { return menuName; }
        }

        private IEnumerable<MenuItem> menuItems;

        private string menuName;
    }
}
