using System;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using Kex.Common;
using Kex.Controller;
using Kex.Model;
using Kex.Model.ItemProvider;
using Microsoft.WindowsAPICodePack.Shell;

namespace Kex.Modell
{
    public class FileItem : IItem<FileProperties>
    {
        private readonly IItemProvider<FileProperties> itemProvider;

        public FileItem(string fullPath, ItemType type, IItemProvider<FileProperties> provider)
        { 
            FullPath = fullPath;  
            ItemType = type;
            itemProvider = provider;
            SetNameFromFullPath();
        }

        public void SetNameFromFullPath()
        {
            var ind = FullPath.LastIndexOf("\\");
            Name = FullPath.Substring(ind != -1 ? ind+1 : 0);
            if (Name == "..")
            {
                var backDir = FullPath.LastIndexOf('\\', ind-1);
                if (backDir > 2)
                    FullPath = FullPath.Substring(0, backDir);
                else
                {
                    FullPath = FullPath.Substring(0, 3);
                }
            }
        }

        public string FullPath { get; set; }
        public string Name { get; set; }
        public ItemType ItemType { get; set; }
        public DateTime? LastModified { get { return Properties == null ? null : Properties.LastModified; } }
        public DateTime? Created { get { return Properties == null ? null : Properties.Created; } }
        public long Length { get { return Properties == null ? 0 : Properties.Length; } }
        public ShellObject ShellObject { get { return Properties == null ? null : Properties.ShellObject; } }
        public BitmapSource Thumbnail { get { return Properties == null ? null : Properties.Thumbnail; } }

        private string _resolvedPath;
        public string ResolvedPath
        {
            get
            {
                return _resolvedPath ?? (_resolvedPath = PathResolver.Resolve(Properties.ShellObject, FullPath)); 
            }
        }   

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnNotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
   
        public FileProperties Properties
        {
            get
            {
                return _properties;
                if (_properties == null)
                {
                    _properties = itemProvider.FetchDetails(this);

                }
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

        private FileProperties _properties;
        private bool _isSelected;

    }
}
