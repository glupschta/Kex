using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Kex.Common;
using Kex.Model;
using Kex.Views;

namespace Kex.Controller.PopupHandler
{
    public class PopupBrowsingHandler : IPopupHandler<IPopupItem>
    {
        private readonly ListboxTextInput textinput;
        public PopupBrowsingHandler(ListboxTextInput textinput)
        {
            this.textinput = textinput;
            textinput.ListSelectionChanged += TextinputListSelectionChanged;
        }

        void TextinputListSelectionChanged(string name)
        {
            if (currentListItems == null) return;
                setSelection(currentListItems.Where(it => it.Name == name).FirstOrDefault());
        }

        public string Name
        {
            get { return "Select"; }
        }

        public IEnumerable<IPopupItem> ListItems
        {
            get
            {
                //ListItems set in TextChanged
                return null;
            }
        }

        public void ItemSelected(IPopupItem item)
        {
        }

        public void HandleKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Enter)
            {
                textinput.IgnoreLostFocus = true;
                ListerManager.Instance.CommandManager.DoDefaultAction();
                textinput.Text = "";
                Keyboard.Focus(textinput.input);
                e.Handled = true;
                textinput.IgnoreLostFocus = false;
            }
        }

        public void TextChanged(string text)
        {
            if (string.IsNullOrEmpty(text)) return;
            var items = ListerManager.Instance.CommandManager.CurrentView.View.Items.Cast<IItem>();
            var matchingStartsWith = items.Where(it => it.Name.StartsWith(text, StringComparison.OrdinalIgnoreCase));
            var matchingContains = items.Where(it => it.Name.ToLower().Contains(text.ToLower()));
            var selection = matchingStartsWith.FirstOrDefault() ?? matchingContains.FirstOrDefault();
            currentListItems = matchingStartsWith.Union(matchingContains);
            ListItems = currentListItems.Select(li => li.Name)
        }

        public bool SetSelectionInListView
        {
            get { return false; }
        }

        private void setSelection(IItem selection)
        {
            if (selection == null) return;
            ListerManager.Instance.CommandManager.CurrentView.View.SelectedItem = selection;
            ListerManager.Instance.CommandManager.CurrentView.View.ScrollIntoView(selection);
            Keyboard.Focus(textinput.input);
        }

        public Func<string, string, bool> Filter
        {
            get { return filter; }
        }

        private bool filter(string source, string text)
        {
            //Filtering done in TextChanged
            return true;
        }

        private IEnumerable<IItem> currentListItems;
    }
}
