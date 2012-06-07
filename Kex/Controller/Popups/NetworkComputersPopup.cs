using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kex.Common;
using Kex.Model;
using Kex.Views;

namespace Kex.Controller.Popups
{
    public class NetworkComputersPopup : BasePopup
    {
        private readonly IEnumerable<IPopupItem> _networkComputers;

        public NetworkComputersPopup(IPopupInput input) : base(input)
        {
            var networkBrowser = new NetworkBrowser();
            _networkComputers = StringPopupItem.GetEnumerable(networkBrowser.GetNetworkComputers());
            ListItems = _networkComputers;
        }

        protected override void HandleSelection()
        {
            var selection = GetSelectedListItem<IPopupItem>();
            if (selection != null)
            {
                ListerManager.Instance.CommandManager.SetContainer(@"\\" + selection.DisplayName);
                Hide();
            }
        }

        protected override void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ApplyListDefaultFilter(_networkComputers);
            Input.ListBox.SelectedIndex = 0;
            base.TextBox_TextChanged(sender, e);
        }

        public override string Name
        {
            get { return "Network"; }
        }
    }
}
