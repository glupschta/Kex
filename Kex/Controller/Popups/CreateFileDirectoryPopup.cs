using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Kex.Common;
using Kex.Model;
using Kex.Views;

namespace Kex.Controller.Popups
{
    public class CreateFileDirectoryPopup : BasePopup
    {
        public CreateFileDirectoryPopup(IPopupInput input) : base(input)
        {
            ListItems = _createTypes.Select(t => new StringPopupItem(t));
            Input.ListBox.SelectedIndex = 0;
        }

        protected override void HandleSelection()
        {
            var dir = ListerManager.Instance.ListerViewManager.CurrentListerView.Lister.CurrentDirectory;
            var path = Path.Combine(dir, Text);
            var selectedType = Input.ListBox.SelectedItem as StringPopupItem;
            if (selectedType.DisplayName == "Directory")
            {
                Directory.CreateDirectory(path);
            }
            else
            {
                File.Create(path);
            }
            //TODO
            ListerManager.Instance.ListerViewManager.CurrentListerView.Lister.Refresh();
            var item = ListerManager.Instance.ListerViewManager.CurrentListerView.Lister.Items.FirstOrDefault(i => i.FullPath == path);
            if (item != null)
                ListerManager.Instance.CommandManager.CurrentItem = item;
                
            ListerManager.Instance.CommandManager.UpdateColumnWidth();
            Hide();
        }


        public override string Name
        {
            get { return "Create"; }
        }

        private static string[] _createTypes = new [] {"Directory","File"};
    }
}
