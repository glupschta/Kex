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
    public class FileItem : IItem, IVirtualizedPropertyProvider<FileProperties>
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
        }

        public string FullPath { get; set; }
        public string Name { get; set; }
        public ItemType ItemType { get; set; }
        public DateTime? LastModified { get { return Properties.LastModified; } }
        public long Length { get { return Properties.Length; } }
        public ShellObject ShellObject { get { return Properties.ShellObject; } }
        public BitmapSource Thumbnail { get { return Properties.Thumbnail; } }

        private string _resolvedPath;
        public string ResolvedPath
        {
            get
            {
                return _resolvedPath ?? (_resolvedPath = PathResolver.Resolve(Properties.ShellObject, FullPath)); 
            }
        }

        
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("IsSelected"));
                }
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
                if (_properties == null)
                {
                    _properties = itemProvider.Fetch(this);
                }
                return _properties;
            }
        }

        public bool Loaded { get; set; }

        private FileProperties _properties;
        private bool _isSelected;

    }
}
