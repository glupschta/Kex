using System.Collections.Generic;
using System.Windows.Input;
using Kex.Common;
using Kex.Interfaces;

namespace Kex.Controller.PopupHandler
{
    public class PopupEnterUrlHandler : IPopupHandler
    {
        public string Name
        {
            get { return "Url"; }
        }

        public MatchMode MatchMode
        {
            get { return MatchMode.Contains; }
        }

        public IEnumerable<string> ListItems
        {
            get
            {
                var history= ListerManager.Manager.CurrentLister.NavigationHistory.Locations;
                history.Reverse();
                return history;
            }
        }

        public void ItemSelected(string item)
        {
            ListerManager.Manager.SetDirectory(item);
        }

        public void HandleKey(object sender, KeyEventArgs e)
        {
        }

        public void TextChanged(string text)
        {
        }
    }
}
