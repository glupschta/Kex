using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Kex.Common;
using Kex.Controller;
using Kex.Model;
using Kex.Model.Item;
using Kex.Model.Lister;

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
            if (Lister != null)
                Lister.SelectionChanged(View, e);
        }

        public ILister Lister
        {
            get { return _lister; }
            set { _lister = value; }
        }


        protected DragDropHandler DragDropHandler { get; set; }
        public ViewHandler ViewHandler { get; protected set; }

        private void ListViewKeyDown(object sender, KeyEventArgs e)
        {
            CommandKeyHandler.HandleKey(e);
        }

        public static T TryFindParent<T>(DependencyObject current) where T : class
        {
            var parent = VisualTreeHelper.GetParent(current);

            if (parent == null) return null;
            if (parent is T) return parent as T;
            return TryFindParent<T>(parent);
        }

        public IEnumerable<IItem> GetSelection()
        {
            return View.SelectedItems.Cast<IItem>();
        }

        private void ViewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (TryFindParent<GridViewColumnHeader>(e.OriginalSource as DependencyObject) == null)
                ListerManager.Instance.CommandManager.DoDefaultAction();
        }

        private void View_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (TryFindParent<GridViewColumnHeader>(e.OriginalSource as DependencyObject) != null) return;

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

        private ILister _lister;


    }

}
