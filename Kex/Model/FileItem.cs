using System;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using Kex.Common;
using Kex.Controller;
using Kex.Model;
using Kex.Model.ItemProvider;
using Kex.Model;
using Microsoft.WindowsAPICodePack.Shell;

namespace Kex.Model
{
    public class FileItem : IItem<FileProperties>, IDisposable
    {
        private readonly IItemProvider<FileProperties> _itemProvider;

        public FileItem(string fullPath, ItemType type, IItemProvider<FileProperties> provider)
        { 
            FullPath = fullPath;  
            ItemType = type;
            _itemProvider = provider;
            SetNameFromFullPath();
        }

        public void SetNameFromFullPath()
        {
            var ind = FullPath.LastIndexOf("\\");
            Name = FullPath.Substring(ind != -1 ? ind+1 : 0);
            if (Name != "..") return;

            var backDir = FullPath.LastIndexOf('\\', ind-1);
            FullPath = FullPath.Substring(0, backDir > 2 ? backDir : 3);
        }

        public string FullPath { get; set; }
        public string Name { get; set; }
        public ItemType ItemType { get; set; }
        public DateTime? LastModified { get { return Properties == null ? null : Properties.LastModified; } }
        public DateTime? Created { get { return Properties == null ? null : Properties.Created; } }
        public long Length { get { return Properties == null ? 0 : Properties.Length; } }
        public ShellObject ShellObject { get { return Properties == null ? null : Properties.ShellObject; } }
        public BitmapSource Thumbnail { get { return Properties == null ? null : Properties.Thumbnail; } } 

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnNotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        private FileProperties _properties;
        public FileProperties Properties
        {
            get
            {
                if (_properties == null)
                    _itemProvider.FetchDetails(this);
                return _properties;
            }
            set { _properties = value; }
        }

        public void PropertiesChanged()
        {
            OnNotifyPropertyChanged("LastModified");
            OnNotifyPropertyChanged("Created");
            OnNotifyPropertyChanged("Length");
            OnNotifyPropertyChanged("ShellObject");
            OnNotifyPropertyChanged("Thumbnail");
            OnNotifyPropertyChanged("Properties");
        }

        public void Dispose()
        {
            Properties.Dispose();
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
