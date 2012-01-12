using System;
using System.Collections.Generic;
using System.Windows.Input;
using Kex.Common;

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
                    ListerManager.Instance.CommandManager.SetView("fullView");
                    break;
                case SimpleView:
                    ListerManager.Instance.CommandManager.SetView("simpleView");
                    break;
                case ThumbView:
                    ListerManager.Instance.CommandManager.SetView("thumbView");
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

        public Func<string, string, bool> Filter
        {
            get { return null; }
        }
    }
}
