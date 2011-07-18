using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Kex.Interfaces;

namespace Kex.Views
{
    /// <summary>
    /// Interaction logic for ListerView.xaml
    /// </summary>
    public partial class ListerView : UserControl
    {
        private readonly CommandKeyHandler keyHandler;
        public ListerView()
        {
            InitializeComponent();
            this.View.SelectionMode = SelectionMode.Single;
            keyHandler = new CommandKeyHandler();
        }


        public ILister Lister { get;  set;}

        private void ListViewKeyDown(object sender, KeyEventArgs e)
        {
            var ignoredKeys = new List<Key> {Key.LeftAlt, Key.LeftCtrl, Key.LeftCtrl, Key.RightAlt, Key.RightCtrl, Key.RightShift};
            if (ignoredKeys.Contains(e.Key)) return;
            bool shift = ((e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift);
            bool control = ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control);
            bool alt = ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt);
            e.Handled = keyHandler.HandleKey(Lister, e.Key, shift, control, alt);
        }

        private void ViewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (TryFindParent<GridViewColumnHeader>(e.OriginalSource as DependencyObject) == null)
                MessageHost.ViewHandler.DoDefaultAction();
        }


        public static T TryFindParent<T>(DependencyObject current) where T : class
        {
            DependencyObject parent = VisualTreeHelper.GetParent(current);

            if (parent == null) return null;
            if (parent is T) return parent as T;
            return TryFindParent<T>(parent);
        }

        private void View_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                MessageHost.ViewHandler.ShowContextMenu();
                e.Handled = true;
            }
            else if (e.ChangedButton == MouseButton.Middle)
            {
                Lister.DirectoryUp();
                e.Handled = true;
            }
        }

        private void DropList_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(dragFormat) ||
                sender == e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void DropList_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(dragFormat))
            {

            }
        }

        private void List_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Store the mouse position
            dragStart = e.GetPosition(null);
        }

        private void List_MouseMove(object sender, MouseEventArgs e)
        {
            // Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Vector diff = dragStart - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                // Get the dragged ListViewItem
                ListView listView = sender as ListView;
                ListViewItem listViewItem = FindAnchestor<ListViewItem>((DependencyObject)e.OriginalSource);

                if (listViewItem != null)
                {
                    // Find the data behind the ListViewItem
                    IItem item = (IItem) listView.ItemContainerGenerator.ItemFromContainer(listViewItem);

                    // Initialize the drag & drop operation
                    
                    DataObject dragData = new DataObject(dragFormat, item.FullPath);
                    DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Move);
                }
            }
        }

        private static T FindAnchestor<T>(DependencyObject current) where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        private Point dragStart;
        readonly string dragFormat = DataFormats.FileDrop;
    }

}
