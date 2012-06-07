using System.Windows;
using System.Windows.Controls;

namespace Kex.Views
{
    public class TileView : ViewBase
    {
        public DataTemplate ItemTemplate { get; set; }

        protected override object DefaultStyleKey
        {
            get { return new ComponentResourceKey(GetType(), "TileView"); }
        }

        protected override object ItemContainerDefaultStyleKey
        {
            get { return new ComponentResourceKey(GetType(), "TileViewItem"); }
        }
    }
}
