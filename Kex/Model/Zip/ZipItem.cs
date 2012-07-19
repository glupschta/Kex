using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using Ionic.Zip;
using Kex.Common;
using Kex.Model.Item;

namespace Kex.Model
{
    public class ZipItem : IItem<FileProperties>
    {
        public ZipItem(ZipEntry zi)
        {
            FullPath = zi.FileName;
            Name = zi.FileName;
            ItemType = zi.IsDirectory ? ItemType.Container : ItemType.Executable;
            DisplayName = zi.FileName;
            FilterString = zi.FileName;
            Properties = new FileProperties();
            Properties.Created = zi.CreationTime;
            Properties.LastModified = zi.LastModified;
            Properties.Length = zi.CompressedSize; 
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
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        public FileProperties Properties
        {
            get; set;
        }
    }
}
