using System;
using System.Collections.Generic;
using System.Windows.Input;
using Kex.Common;
using System.Linq;

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
                var history = ListerManager.Instance.CommandManager.CurrentView.Lister.NavigationHistory.Locations;
                history.Reverse();
                return history.Select(h => h.FullPath);
            }
        }

        public void ItemSelected(string item)
        {
            ListerManager.Instance.CommandManager.SetContainer(item);
        }

        public void HandleKey(object sender, KeyEventArgs e)
        {
        }

        public void TextChanged(string text)
        {
        }

        public Func<string, string, bool> Filter
        {
            get { return null; }
        }
    }
}
