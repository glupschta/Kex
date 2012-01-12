using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Kex.Common;
using Kex.Controller;
using Kex.Model;

namespace Kex.Views
{
    /// <summary>
    /// Interaction logic for ListerView.xaml
    /// </summary>
    public partial class ListerView : UserControl
    {
        public ListerView()
        {
            InitializeComponent();
            this.View.SelectionMode = SelectionMode.Single;
            this.DragDropHandler = new DragDropHandler(this);
        }

        public ILister<FileProperties> Lister { get;  set;}
        protected DragDropHandler DragDropHandler { get; set; }

        private void ListViewKeyDown(object sender, KeyEventArgs e)
        {
            var ignoredKeys = new List<Key> {Key.LeftAlt, Key.LeftCtrl, Key.LeftCtrl, Key.RightAlt, Key.RightCtrl, Key.RightShift};
            if (ignoredKeys.Contains(e.Key)) return;
            bool shift = ((e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift);
            bool control = ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control);
            bool alt = ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt);
            e.Handled = CommandKeyHandler.HandleKey(e.Key, shift, control, alt);
        }

        private void ViewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (TryFindParent<GridViewColumnHeader>(e.OriginalSource as DependencyObject) == null)
                ListerManager.Instance.CommandManager.DoDefaultAction();
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
                ListerManager.Instance.CommandManager.ShowContextMenu();
                e.Handled = true;
            }
            else if (e.ChangedButton == MouseButton.Middle)
            {
                Lister.DirectoryUp();
                e.Handled = true;
            }
        }


    }

}
