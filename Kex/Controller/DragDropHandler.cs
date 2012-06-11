using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Kex.Model;
using Kex.Views;

namespace Kex.Controller
{
    public class DragDropHandler
    {
        private readonly ListerView listerView;

        public DragDropHandler(ListerView listerView)
        {
            this.listerView = listerView;
            registerEvents();
        }

        private void registerEvents()
        {
            listerView.View.Drop += DropList_Drop;
            listerView.View.DragEnter += DropList_DragEnter;
            listerView.View.PreviewMouseLeftButtonDown += List_PreviewMouseLeftButtonDown;
            listerView.View.PreviewMouseMove += List_MouseMove;
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
                var dirNames = (string[])e.Data.GetData(DataFormats.FileDrop);
                var firstName = dirNames.First();
                string destination = null;

                var listViewItem = FindAnchestor<ListViewItem>(e.OriginalSource);
                if (listViewItem == null)
                {
                    var listerView = FindAnchestor<ListerView>(e.OriginalSource);
                    destination = listerView.Lister.CurrentDirectory;
                }
                else
                {
                    var item = ((IItem) listViewItem.Content);
                    destination = item.FullPath;
                }
                if (firstName != destination)
                {
                    MessageBox.Show("Dropping " + firstName + " to " + destination);
                }
            }
        }

        private void List_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dragStart = e.GetPosition(listerView);
        }

        private void List_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(null);
            Vector diff = dragStart - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                var listView = sender as ListView;
                var listViewItem = FindAnchestor<ListViewItem>((DependencyObject)e.OriginalSource);

                if (listViewItem != null)
                {
                    var item = (IItem)listView.ItemContainerGenerator.ItemFromContainer(listViewItem);

                    var dragData = new DataObject(dragFormat, new[] { item.FullPath });
                    DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Move | DragDropEffects.Copy | DragDropEffects.Link);
                }
            }
        }

        public static T FindAnchestor<T>(object obj) where T : DependencyObject
        {
            var current = obj as DependencyObject;
            do
            {
                if (current is T) return (T)current;
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        private Point dragStart;
        readonly string dragFormat = DataFormats.FileDrop;
    }
}
