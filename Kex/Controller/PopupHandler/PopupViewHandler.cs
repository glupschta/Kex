using System.Collections.Generic;
using System.Windows.Input;
using Kex.Common;
using Kex.Interfaces;

namespace Kex.Controller.PopupHandler
{
    public class PopupViewHandler : IPopupHandler
    {
        private const string FullView = "Full";
        private const string SimpleView = "Simple";
        private const string ThumbView = "Thumbs";

        public string Name
        {
            get { return "Views"; }
        }

        public MatchMode MatchMode
        {
            get { return MatchMode.StartsWith; }
        }

        public IEnumerable<string> ListItems
        {
            get { return new List<string> { FullView, SimpleView, ThumbView }; }
        }

        public void ItemSelected(string item)
        {
            switch (item)
            {
                case FullView:
                    MessageHost.ViewHandler.SetView("fullView");
                    break;
                case SimpleView:
                    MessageHost.ViewHandler.SetView("simpleView");
                    break;
                case ThumbView:
                    MessageHost.ViewHandler.SetView("thumbView");
                    break;
                default:
                    break;
            }
        }

        public void HandleKey(object sender, KeyEventArgs e)
        {
        }

        public void TextChanged(string text)
        {
        }
    }
}
