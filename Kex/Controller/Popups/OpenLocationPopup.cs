using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Kex.Common;
using Kex.Model;
using Kex.Model.Item;
using Kex.Model.ItemProvider;
using Kex.Views;

namespace Kex.Controller.Popups
{
    public class OpenLocationPopup : BasePopup
    {
        public OpenLocationPopup(IPopupInput input, bool newWindow) : base(input)
        {
            ListItems = LocationHistory;
            NewWindow = newWindow;
        }

        protected override void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Input.ListBox.SelectedIndex = -1;
            ApplyListDefaultFilter(LocationHistory);
        }

        protected override void HandleSelection()
        {
            if (Input.ListBox.SelectedIndex != -1)
                SelectedLocation = ((StringPopupItem)Input.ListBox.SelectedItem).FilterString;
            else
            {
                SelectedLocation = Input.TextBox.Text;
                if (string.IsNullOrEmpty(SelectedLocation))
                {
                    NewWindow = true;
                    SelectedLocation =  ListerManager.Instance.ListerViewManager.CurrentListerView.Lister.CurrentDirectory;
                }
            }

            if (File.Exists(SelectedLocation))
            {
                SelectedLocation = new FileInfo(SelectedLocation).FullName;
                FileItem fi = new FileItem(SelectedLocation, ItemType.Executable, null);// ListerManager.Instance.ListerViewManager.CurrentListerView.Lister.ItemProvider as FilesystemItemProvider);
                ListerManager.Instance.CommandManager.DoDefaultAction();
            }
            else if (Directory.Exists(SelectedLocation))
            {
                SelectedLocation = new DirectoryInfo(SelectedLocation).FullName;
                if (NewWindow)
                    ListerManager.Instance.ListerViewManager.OpenLister(SelectedLocation);
                else
                    ListerManager.Instance.CommandManager.SetContainer(SelectedLocation);
            }
            Hide();
        }

        public override string Name
        {
            get { return NewWindow ? "Open New Lister" : "Open"; }
        }

        public IEnumerable<StringPopupItem> LocationHistory
        {
            get
            {
                var historyItems =ListerManager.Instance.ListerViewManager.CurrentListerView.Lister.NavigationHistory.Locations.Values;
                return historyItems.Select(hi => new StringPopupItem(hi.FullPath));
            }
        }

        public string SelectedLocation { get; private set; }
        public bool NewWindow { get; private set; }
    }
}
