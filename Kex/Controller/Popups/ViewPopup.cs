using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kex.Common;
using Kex.Views;

namespace Kex.Controller.Popups
{
    public class ViewPopup :BasePopup
    {
        public ViewPopup(IPopupInput input) : base(input)
        {
            ListItems = Views;
        }

        protected override void HandleSelection()
        {
            var selection = GetSelectedListItem<IPopupItem>();
            if (selection == null) return;
            switch (selection.FilterString)
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
            Hide();
        }

        protected override void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ApplyListDefaultFilter(Views);
            Input.ListBox.SelectedIndex = 0;
            base.TextBox_TextChanged(sender, e);
        }

        public override string Name
        {
            get { return "Views"; }
        }

        public IEnumerable<IPopupItem> Views
        {
            get
            {
                var views = new List<string> { ViewHandler.FullView, ViewHandler.SimpleView, ViewHandler.ThumbView };
                return StringPopupItem.GetEnumerable(views);
            }
        }
    }
}
