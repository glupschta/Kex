using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Kex.Common;
using Kex.Controller;
using Kex.Model;
using Path = System.IO.Path;

namespace Kex.Views
{
    /// <summary>
    /// Interaction logic for RenamePopup.xaml
    /// </summary>
    public partial class RenamePopup : UserControl
    {
        public RenamePopup()
        {
            InitializeComponent();
            renameTextBox.FontFamily = Options.FontFamily;
            renameTextBox.FontSize = Options.FontSize;
            FontFamily = Options.FontFamily;
            FontSize = Options.FontSize;

            var typeFace = new Typeface(Options.FontFamily.ToString());
            var ft = new FormattedText(" ", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeFace, Options.FontSize, Brushes.Black);
            renameTextBox.Height = ft.Height + 6;

            KeyDown += RenamePopup_KeyDown;
            renamePopup.Closed += renamePopup_Closed;
        }

        void renamePopup_Closed(object sender, EventArgs e)
        {
            ListerManager.Instance.CommandManager.FocusView();
        }

        void doRename()
        {
            var item = currentView.View.SelectedItem as FileItem;
            if (item != null)
            {
                if (item.ItemType == ItemType.Container)
                {
                    var di = new DirectoryInfo(item.FullPath);
                    var dest = Path.Combine(di.Root.FullName, renameTextBox.Text);
                    di.MoveTo(dest);
                    item.FullPath = dest;
                }
                else
                {
                    var di = new FileInfo(item.FullPath);
                    var dest = Path.Combine(di.Directory.Root.FullName, renameTextBox.Text);
                    di.MoveTo(dest);
                    item.FullPath = dest;
                }
                item.Name = renameTextBox.Text;
                item.OnNotifyPropertyChanged("Name");
                item.OnNotifyPropertyChanged("FullName");
            }
        }

        void RenamePopup_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Return:
                    doRename();
                    renamePopup.IsOpen = false;
                    e.Handled = true;
                    break;
                case Key.Escape:
                    e.Handled = true;
                    renamePopup.IsOpen = false;
                    break;
            }
        }

        public void Show(ListerView view)
        {
            currentView = view;
            renamePopup.Placement = PlacementMode.Relative;
            var target = view.View.ItemContainerGenerator.ContainerFromItem(view.View.SelectedItem) as UIElement;
            renamePopup.PlacementTarget = target;
            renamePopup.HorizontalOffset = 31;
            renamePopup.VerticalOffset = -1;
            renamePopup.IsOpen = true;
            renameTextBox.Text = ((IItem)view.View.SelectedItem).Name;
            renameTextBox.Focus();
            var endIndex = renameTextBox.Text.LastIndexOf('.');
            if (endIndex == -1)
                endIndex = renameTextBox.Text.Length;
            renameTextBox.SelectionStart = 0;
            renameTextBox.SelectionLength = endIndex;
        }

        private ListerView currentView;

    }
}
