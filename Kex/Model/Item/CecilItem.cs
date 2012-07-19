using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace Kex.Model.Item
{
    public class CecilItem : IItem
    {
        public CecilItem()
        {
            
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public string DisplayName { get; private set; }
        public string FilterString { get; private set; }
        public BitmapSource Thumbnail { get; private set; }
        public string FullPath { get; set; }
        public string Name { get; set; }
        public ItemType ItemType { get; set; }
        public void OnNotifyPropertyChanged(string property)
        {
        }
    }
}
