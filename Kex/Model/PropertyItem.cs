using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace Kex.Model
{
    public class PropertyItem : IItem
    {
        public  PropertyItem(string key, string value, ItemType type)
        {
            Name = FilterString = DisplayName = key;
            Value = FullPath = value;
            Properties1 = new PropertyProperties(value); 
            ItemType = type;
            Childs = new List<PropertyItem>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public string DisplayName { get; private set; }
        public string FilterString { get; private set; }
        public BitmapSource Thumbnail { get; private set; }
        public string FullPath { get; set; }
        public string Name { get; set; }

        public string Value { get; set; }
        public ItemType ItemType { get; set; }

        public List<PropertyItem> Childs { get; set; }

        public PropertyProperties Properties1 { get; set; }

        public void OnNotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }

    public class PropertyProperties
    {
        public PropertyProperties(string val)
        {
            LastModified = val;
        }
        public string LastModified { get; set; }
    }
}
