using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Kex.Common;
using Kex.Model;
using Kex.Model.ItemProvider;
using Kex.Model.Lister;
using Kex.Views;
using Microsoft.WindowsAPICodePack.Shell;

namespace Kex.Controller.Popups
{
    public class DrivesPopup : BasePopup
    {
        public DrivesPopup(IPopupInput input) : base(input)
        {
            ListItems = DriveItem.GetAllItems();
        }

        protected override void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ListItems = new ItemFilter(DriveItem.GetAllItems(), Text);
            Input.ListBox.SelectedIndex = 0;
        }

        protected override void HandleSelection()
        {
            var selectedDrive = Input.ListBox.SelectedItem as DriveItem;
            if (selectedDrive != null)
            {
                if (!(ListerManager.Instance.ListerViewManager.CurrentListerView.Lister is FileLister))
                {

                    var lister = new FileLister();
                    var view = ListerManager.Instance.ListerViewManager.CurrentListerView;
                    view.Lister = lister;
                    view.DataContext = lister;
                }
                ListerManager.Instance.CommandManager.SetContainer(selectedDrive.RootDirectory);
                Hide();
            }
        }

        public override string Name
        {
            get { return "Drives"; }
        }
    }

    public class DriveItem : IPopupItem
    {

        public DriveItem(DriveInfo info)
        {
            Name = GetDriveInfoString(info);
            RootDirectory = info.RootDirectory.FullName;
            ShellObject = ShellObject.FromParsingName(RootDirectory);
        }

        public static List<DriveItem> GetAllItems()
        {
            return drives ?? (drives = DriveInfo.GetDrives().Select(d => new DriveItem(d)).ToList());
        }

        private static List<DriveItem> drives;

        public static string GetDriveInfoString(DriveInfo di)
        {
            if (!di.IsReady) 
                return string.Format("{0} - not ready", di.Name);
            try
            {
                return string.Format("{0} {1} ({2}/{3} GB free)", di.Name, di.VolumeLabel, di.TotalFreeSpace / 1000000000, di.TotalSize / 1000000000);
            }
            catch
            {
                return di.Name;
            }
        }

        public string Name { get; set; }
        public string RootDirectory { get; set; }
        private ShellObject ShellObject;

        public string DisplayName
        {
            get { return Name; }
        }

        public string FilterString
        {
            get { return Name; }
        }

        public BitmapSource Thumbnail {
            get
            {
                if (ShellObject == null)
                    return null;
                return ShellObject.Thumbnail.SmallBitmapSource;
            }
        }
    }
}
