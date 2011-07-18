using System.Collections.Generic;
using System.Windows.Input;
using Kex.Interfaces;

namespace Kex.Controller.PopupHandler
{
    public class PopupFilterHandler : IPopupHandler
    {
        public string Name
        {
            get { return "Filter"; }
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
        }

        public void TextChanged(string text)
        {
            MessageHost.ViewHandler.SetFilter(text);
        }
    }
}
