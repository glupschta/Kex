using System;
using System.Collections.Generic;
using System.Windows.Input;
using Kex.Common;

namespace Kex.Controller.PopupHandler
{
    public class PopupViewHandler : IPopupHandler<string>
    {

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
            get { return new List<string> { ViewHandler.FullView, ViewHandler.SimpleView, ViewHandler.ThumbView }; }
        }

        public void ItemSelected(string item)
        {
            switch (item)
            {
                case ViewHandler.FullView:
                    ListerManager.Instance.CommandManager.SetView("fullView");
                    break;
                case ViewHandler.SimpleView:
                    ListerManager.Instance.CommandManager.SetView("simpleView");
                    break;
                case ViewHandler.ThumbView:
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

        public bool SetSelectionInListView
        {
            get { return true; }
        }

        public Func<string, string, bool> Filter
        {
            get { return null; }
        }
    }
}
