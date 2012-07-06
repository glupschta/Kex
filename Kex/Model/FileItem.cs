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
    public class FileItem : IItem, IDisposable
    {
        private readonly FilesystemItemProvider _itemProvider;

        public FileItem(string fullPath, ItemType type, FilesystemItemProvider provider)
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
        public ShellObject ShellObject { get { return Properties == null ? null : Properties.ShellObject; } }
        public BitmapSource Thumbnail { get { return Properties == null ? null : Properties.Thumbnail; } } 

        public void FetchDetails()
        {
            var props = new FileProperties();
            props.ShellObject = ShellObject.FromParsingName(FullPath);
            props.Length = (long)(props.ShellObject.Properties.System.Size.Value ?? 0);
            props.Created = props.ShellObject.Properties.System.DateCreated.Value;
            props.LastModified = props.ShellObject.Properties.System.DateCreated.Value;

            props.ShellObject.Thumbnail.FormatOption = ShellThumbnailFormatOption.Default;
            props.ShellObject.Thumbnail.RetrievalOption = ShellThumbnailRetrievalOption.Default;
            props.Thumbnail = props.ShellObject.Thumbnail.MediumBitmapSource;
            //props.Thumbnail.Freeze();
            Properties = props;
        } 

        private FileProperties _properties;
        public FileProperties Properties
        {
            get
            {
                if (_properties == null)
                    FetchDetails();
                return _properties;
            }
            set { _properties = value; }
        }

        public void PropertiesChanged()
        {
            OnNotifyPropertyChanged("Properties");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnNotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
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
