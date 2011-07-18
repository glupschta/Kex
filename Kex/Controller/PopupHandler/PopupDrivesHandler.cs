using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Kex.Interfaces;

namespace Kex.Controller.PopupHandler
{
    public class PopupDrivesHandler : IPopupHandler
    {
        private readonly Dictionary<string, string> drives;

        public PopupDrivesHandler()
        {
            drives = DriveInfo.GetDrives().ToDictionary(GetDriveInfoString, d => d.RootDirectory.FullName);
        }

        public string GetDriveInfoString(DriveInfo di)
        {
            try
            {
                return string.Format("{0} {1} ({2}/{3} GB free)", di.Name, di.VolumeLabel, di.TotalFreeSpace/1000000000, di.TotalSize/1000000000);
            } catch
            {
                return di.Name;
            }
        }

        public string Name
        {
            get { return "Drives"; }
        }

        public MatchMode MatchMode
        {
            get { return MatchMode.StartsWith; }
        }

        public IEnumerable<string> ListItems
        {
            get { return drives.Keys; }
        }

        public void ItemSelected(string item)
        {
            ListerManager.Manager.SetDirectory(drives[item]);
        }

        public void HandleKey(object sender, KeyEventArgs e)
        {
        }

        public void TextChanged(string text)
        {
        }
    }
}
