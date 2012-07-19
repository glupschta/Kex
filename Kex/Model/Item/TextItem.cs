using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace Kex.Model.Item
{
    public class TextItem : IItem
    {
        public TextItem(int lineNumber, string text)
        {
            DisplayName = text;
            FilterString = text;
            LineNumber = lineNumber.ToString("00000");
            ItemType = ItemType.Executable;
            Name = LineNumber;
        }

        public string LineNumber { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public string DisplayName { get; private set; }
        public string FilterString { get; private set; }
        public BitmapSource Thumbnail { get; private set; }
        public string FullPath { get; set; }
        public string Name { get; set; }
        public ItemType ItemType { get; set; }
        public void OnNotifyPropertyChanged(string property)
        {
        }
    }
}
