using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Kex.Interfaces;
using Microsoft.WindowsAPICodePack.Shell;

namespace Kex
{
    public class Item : IItem
    {
        public Item(string fullPath, ItemType type)
        { 
            FullPath = fullPath;  
            Type = type;
            SetNameFromFullPath();
        }

        public void SetNameFromFullPath()
        {
            var ind = FullPath.LastIndexOf("\\");
            Name = FullPath.Substring(ind != -1 ? ind+1 : 0);
        }

        public string FullPath { get; set; }
        public string Name { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? LastModified { get; set; }
        public long Length { get; set; }
        public ItemType Type { get; set; }
        public ShellObject ShellObject { get; set; }

        public BitmapSource Thumbnail { get; set; }

        private string _resolvedPath;
        public string ResolvedPath
        {
            get { return _resolvedPath ?? (_resolvedPath = PathResolver.Resolve(ShellObject, FullPath)); }
        }


        private bool _isSelected;
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
       
    }
}
