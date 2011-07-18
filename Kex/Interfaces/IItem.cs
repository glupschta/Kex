using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Kex.Modell;
using Microsoft.WindowsAPICodePack.Shell;

namespace Kex.Interfaces
{
    public interface IItem : INotifyPropertyChanged
    {
        string FullPath { get; set; }
        string Name { get; set; }
        DateTime? Created { get; set; }
        DateTime? LastModified { get; set; }
        long Length { get; set; }
        ItemType Type { get; set; }
        bool IsSelected { get; set; }
        ShellObject ShellObject { get; set; }
        string ResolvedPath { get; }
        void OnNotifyPropertyChanged(string property);
        BitmapSource Thumbnail { get; set; }
    }
}
