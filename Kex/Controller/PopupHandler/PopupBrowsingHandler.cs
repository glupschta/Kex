using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Kex.Common;
using Kex.Interfaces;
using Kex.Views;

namespace Kex.Controller.PopupHandler
{
    public class PopupBrowsingHandler : IPopupHandler
    {
        private readonly ListboxTextInput textinput;
        public PopupBrowsingHandler(ListboxTextInput textinput)
        {
            this.textinput = textinput;
        }

        public string Name
        {
            get { return "Select"; }
        }

        public MatchMode MatchMode
        {
            get { return MatchMode.StartsWith; }
        }

        public IEnumerable<string> ListItems
        {
            get { return null; }
        }

        public void ItemSelected(string item)
        {
        }

        public void HandleKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Enter)
            {
                textinput.ignoreLostFocus = true;
                ListerManager.Manager.DoDefaultAction();
                textinput.Text = "";
                Keyboard.Focus(textinput.input);
                e.Handled = true;
                textinput.ignoreLostFocus = false;
            }
        }

        public void TextChanged(string text)
        {
            var items = ListerManager.Manager.CurrentView.View.Items.Cast<IItem>();
            IItem selection;
            if (text.Length > 1 && text.Contains(","))
            {
                var regEx = new Regex("^" + text.Replace(",", ".*"), RegexOptions.IgnoreCase);
                selection = items.Where(it => regEx.IsMatch(it.Name)).FirstOrDefault();
            }
            else
            {
                selection=items.Where(it => it.Name.StartsWith(text, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            }
            if (selection != null)
            {
                ListerManager.Manager.CurrentView.View.SelectedItem = selection;
                Keyboard.Focus(textinput.input);
            }
        }
    }
}
