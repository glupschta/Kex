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
using Kex.Model.Item;
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

            KeyDown += RenamePopup_KeyDown;
            renamePopup.Closed += renamePopup_Closed;
            renameTextBox.TextChanged += renameTextBox_TextChanged;
        }

        void renameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var typeFace = new Typeface(Options.FontFamily.ToString());
            var ft = new FormattedText(renameTextBox.Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeFace, Options.FontSize, Brushes.Black);
            if (ft.Width + 20 > originalSize)
                renameTextBox.Width = ft.Width + 20;
        }

        void renamePopup_Closed(object sender, EventArgs e)
        {
            ListerManager.Instance.CommandManager.FocusView();
        }

        void doRename()
        {
            try {
                var item = currentView.View.SelectedItem as FileItem;
                if (item != null)
                {
                    if (item.ItemType == ItemType.Container)
                    {
                        var di = new DirectoryInfo(item.FullPath);
                        var parent = di.Parent ?? di.Root;
                        var dest = Path.Combine(parent.FullName, renameTextBox.Text);
                        if (item.FullPath.Equals(dest, StringComparison.OrdinalIgnoreCase))
                            return;
                        di.MoveTo(dest);
                        item.FullPath = dest;
                    }
                    else
                    {
                        var di = new FileInfo(item.FullPath);
                        var parent = di.Directory;
                        var dest = Path.Combine(parent.FullName, renameTextBox.Text);
                        if (item.FullPath.Equals(dest, StringComparison.OrdinalIgnoreCase))
                            return;
                        di.MoveTo(dest);
                        item.FullPath = dest;
                    }
                    item.Name = renameTextBox.Text;
                    item.OnNotifyPropertyChanged("Name");
                    item.OnNotifyPropertyChanged("FullPath");

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.ShowError(ex);
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
                case Key.Oem102:
                case Key.Escape:
                    e.Handled = true;
                    renamePopup.IsOpen = false;
                    break;
            }
        }

        public void Show(ListerView view)
        {
            currentView = view;
            var gridView = view.View.View as GridView;

            UIElement target = null;
            double nameColumnPosition = 0;
            
            if (gridView != null)
            {
                var nameColumnFound = false;
                foreach(var col in gridView.Columns)
                {
                    if ("Name" == col.Header as string)
                    {
                        nameColumnFound = true;
                        break;
                    }
                    nameColumnPosition += col.ActualWidth;
                }
                if (!nameColumnFound)
                {
                    var firstColumn = gridView.Columns.FirstOrDefault();
                    nameColumnPosition = gridView.Columns.First().Width;
                }
            }
            else
            {
                nameColumnPosition = 31;
            }
            target = view.View.ItemContainerGenerator.ContainerFromItem(view.View.SelectedItem) as UIElement;
            renamePopup.PlacementTarget = target;
            renamePopup.Placement = PlacementMode.Relative;
            renamePopup.HorizontalOffset = nameColumnPosition + 3;
            renamePopup.VerticalOffset = -1;

            renameTextBox.Text = ((IItem)view.View.SelectedItem).Name;
            var typeFace = new Typeface(Options.FontFamily.ToString());
            var ft = new FormattedText(renameTextBox.Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeFace, Options.FontSize, Brushes.Black);
            renameTextBox.Height = ft.Height + 6;
            renameTextBox.Width = ft.Width + 20;
            originalSize = ft.Width + 20;

            renamePopup.IsOpen = true;

            renameTextBox.Focus();
            var endIndex = renameTextBox.Text.LastIndexOf('.');

            var item = view.View.SelectedItem as FileItem;
            var isDirectory = false;
            if (item != null)
            {
                isDirectory = item.ItemType == ItemType.Container;
            }

            if (endIndex == -1 || isDirectory)
                endIndex = renameTextBox.Text.Length;
            renameTextBox.SelectionStart = 0;
            renameTextBox.SelectionLength = endIndex;
        }

        private ListerView currentView;
        private double originalSize;

    }
}
