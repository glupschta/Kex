using System;
using System.Windows.Controls;
using System.Windows.Input;
using Kex.Common;

namespace Kex.Views
{
    /// <summary>
    /// Interaction logic for PathHeader.xaml
    /// </summary>
    public partial class PathHeader : UserControl
    {
        public PathHeader()
        {
            InitializeComponent();
            listView.SelectionChanged += ListViewSelectionChanged;
            listView.MouseDown += new MouseButtonEventHandler(listView_MouseDown);
        }

        void listView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void ListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PathPartSelected != null && listView.SelectedItem != null)
            {
                PathPartSelected(listView.SelectedItem as PathPart);
            }
        }

        public event Action<PathPart> PathPartSelected;

    }
}
