using System.Windows.Controls;
using Kex.Common;

namespace Kex.Views
{
    /// <summary>
    /// Interaction logic for ListerStatusbar.xaml
    /// </summary>
    public partial class ListerStatusbar : UserControl
    {
        public ListerStatusbar()
        {
            InitializeComponent();
            Panel.FontFamily = Options.FontFamily;
            Panel.FontSize = Options.FontSize;
            Panel.Height = Options.FontSize*2;
        }
    }
}
