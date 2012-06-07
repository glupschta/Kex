using System;
using System.Collections.Generic;
using System.Linq;
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
            DragDropHandler = new DragDropHandler(this);
            ViewHandler = new ViewHandler(this);
            View.SelectionChanged += View_SelectionChanged;
        }

        void View_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Lister.SelectionChanged(this.View, e);
        }

        public ILister<FileProperties> Lister { get;  set;}
        protected DragDropHandler DragDropHandler { get; set; }
        public ViewHandler ViewHandler { get; protected set; }

        private void ListViewKeyDown(object sender, KeyEventArgs e)
        {
            CommandKeyHandler.HandleKey(e);
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

        public IEnumerable<IItem> GetSelection()
        {
            return View.SelectedItems.Cast<IItem>();
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
                Lister.ContainerUp();
                e.Handled = true;
            }
        }


    }

}
