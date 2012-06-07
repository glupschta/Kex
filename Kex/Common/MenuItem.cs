using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace Kex.Common
{
    public class MenuItem : IPopupItem
    {
        public MenuItem(MenuItem parent, string displayText, string command, string argument = null)
        {
            Parent = parent;
            Name = displayText;
            Command = command;
            Argument = argument;
            SubItems = new List<MenuItem>();
        }

        public string Name { get; private set; }
        public MenuItem Parent { get; private set; }
        public List<MenuItem> SubItems { get; private set; }
        public string Command { get; set; }
        public string Argument { get; set; }
        public BitmapSource Thumbnail { get; set; }

        public bool HasChilds
        {
            get { return SubItems.Any(); }
        }

        public string DisplayName
        {
            get { return Name; }
        }

        public string FilterString
        {
            get { return Name; }
        }

    }
}
