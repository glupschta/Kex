using System.ComponentModel;
using Kex.Common;

namespace Kex.Model.Item
{
    public interface IItem : INotifyPropertyChanged, IPopupItem
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
