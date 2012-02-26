using System;
using System.Collections.Generic;
using System.Windows.Input;
using Kex.Common;
using System.Linq;
using Kex.Modell;
using Kex.Views;

namespace Kex.Controller.PopupHandler
{
    public class PopupFilterHandler : IPopupHandler<string>
    {
        public string Name
        {
            get { return "Filter"; }
        }

        private readonly ListboxTextInput textinput;
        public PopupFilterHandler(ListboxTextInput textinput)
        {
            this.textinput = textinput;
        }


        public IEnumerable<string> ListItems
        {
            get
            {
                var types = ListerManager.Instance.ListerViewManager.CurrentListerView.Lister.Items.Cast<FileItem>()
                    .Select(fi => fi.Properties.ShellObject.Properties.System.FileExtension.Value)
                    .Where(ext => !string.IsNullOrEmpty(ext))
                    .Distinct();
                return types;
            }
        }

        public void ItemSelected(string item)
        {
            if (textinput.listView.SelectedIndex > -1)
            {
                ListerManager.Instance.CommandManager.SetFilter((string)textinput.listView.SelectedValue);
            }
            else
            {
                ListerManager.Instance.ListerViewManager.CurrentListerView.View.SelectedIndex = 0;
            }
        }

        public void HandleKey(object sender, KeyEventArgs e)
        {
        }

        public void TextChanged(string text)
        {
            ListerManager.Instance.CommandManager.SetFilter(text);
        }

        public bool SetSelectionInListView
        {
            get { return false; }
        }

        public Func<string, string, bool> Filter
        {
            get { return null; }
        }
    }
}
