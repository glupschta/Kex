using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Kex.Model;

namespace Kex.Controller.PopupHandler
{
    public class PopupNetworkComputersHandler : IPopupHandler<string>
    {
        public string Name
        {
            get { return "Network"; }
        }

        public IEnumerable<string> ListItems
        {
            get 
            { 
                var browser = new NetworkBrowser();
                return browser.GetNetworkComputers();
            }
        }

        public Func<string, string, bool> Filter
        {
            get { return null; }
        }

        public void ItemSelected(string item)
        {
            ListerManager.Instance.CommandManager.SetContainer(@"\\"+item);
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
