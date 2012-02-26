using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

namespace Kex.Controller.PopupHandler
{
    public class ShellPropertyPopupHandler : IPopupHandler<string>
    {
        public string Name
        {
            get { return "ShellProperties"; }
        }

        public IEnumerable<string> ListItems
        {
            get
            {
                var so = ShellObject.FromParsingName(@"C:\");
                return so.Properties.DefaultPropertyCollection.Select(co => co.CanonicalName);
            }
        }

        public Func<string, string, bool> Filter
        {
            get { return null; }
        }

        public void ItemSelected(string item)
        {
        }

        public void HandleKey(object sender, KeyEventArgs e)
        {
        }

        public void TextChanged(string text)
        {
        }

        public bool SetSelectionInListView
        {
            get { return true; }
        }
    }
}
