using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Kex.Common;

namespace Kex.Controller.PopupHandler
{
    public class PopupDrivesHandler : IPopupHandler
    {
        private static Dictionary<string, string> drives;
        private static Dictionary<string, string> Drives
        {
            get
            {
                if (drives == null)
                {
                    drives = DriveInfo.GetDrives().ToDictionary(GetDriveInfoString, d => d.RootDirectory.FullName);
                }
                return drives;
            }
        }

        public static string GetDriveInfoString(DriveInfo di)
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
            get { return Drives.Keys; }
        }

        public void ItemSelected(string item)
        {
            ListerManager.Manager.SetDirectory(Drives[item]);
        }

        public void HandleKey(object sender, KeyEventArgs e)
        {
        }

        public void TextChanged(string text)
        {
        }
    }
}
