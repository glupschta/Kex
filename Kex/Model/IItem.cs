using System;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using Kex.Modell;
using Microsoft.WindowsAPICodePack.Shell;

namespace Kex.Model
{
    public interface IItem : INotifyPropertyChanged
    {
        string FullPath { get; set; }
        string Name { get; set; }
        ItemType ItemType { get; set; }
        void OnNotifyPropertyChanged(string property);
    }

    public interface IItem<T> : IItem
    {
        T Properties { get; set; }
    }
}
