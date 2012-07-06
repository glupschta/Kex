using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using Kex.Common;
using Kex.Model;
using Kex.Model.ItemProvider;
using Kex.Model;
using Kex.Model.Lister;
using Kex.Views;
using ListView = System.Windows.Controls.ListView;
using ListViewItem = System.Windows.Controls.ListViewItem;
using TabControl = System.Windows.Controls.TabControl;

namespace Kex.Controller
{
    public class TabbedListerViewManager : IListerViewManager
    {
        public TabbedListerViewManager(TabControl tabControl, ListboxTextInput textInput)
        {
            _tabControl = tabControl;
            TextInput = textInput;
        }

        public ListerView CurrentListerView { get; set; }

        public void OpenLister(string container)
        {
            var lister = new FileLister();
            OpenLister(container, lister);
        }

        public void OpenLister(string container, ILister lister)
        {
            lister.CurrentDirectory = container;
            var listerView = new ListerView { Lister = lister, DataContext = lister };
            lister.ListView = listerView.View;
            var newTab = new TabItem();
            newTab.Header = container;
            newTab.Content = listerView;
            _tabControl.Items.Add(newTab);

            CurrentListerView = listerView;

            if (lister.XamlView == null)
                setGridViewColumns(listerView.View, lister);
            else
                SetView(lister.XamlView);

            listerView.GotFocus += ListerViewGotFocus;
            listerView.View.Loaded += SetFocus;
            listerView.Lister.PropertyChanged += ListerPropertyChanged;
            _tabControl.SelectedItem = newTab;
        }

        private void setGridViewColumns(ListView listView, ILister lister)
        {
            GridView myGridView = new GridView();
            myGridView.AllowsColumnReorder = true;

            foreach (var col in lister.Columns)
            {
                GridViewColumn gc = new GridViewColumn();
                gc.DisplayMemberBinding = new Binding(col.BindingExpression);
                gc.Header = col.Header;
                gc.Width = double.NaN;
                myGridView.Columns.Add(gc);
            }

            listView.View = myGridView;
        }

        public void CloseCurrentLister()
        {
            if (_tabControl.Items.Count == 1) return;
            var ind = _tabControl.SelectedIndex;
            _tabControl.Items.Remove(_tabControl.SelectedItem);
            if (ind > _tabControl.Items.Count -1)
            {
                ind = _tabControl.Items.Count - 1;
            }
            CurrentListerView = ((TabItem) _tabControl.Items[ind]).Content as ListerView;
            SetFocus(null, null);
        }

        public void CycleListers(int direction)
        {
            var ind = _tabControl.SelectedIndex;
            ind += direction;
            if (ind < 0)
            {
                ind = _tabControl.Items.Count - 1;
            }
            else if (ind > _tabControl.Items.Count - 1)
            {
                ind = 0;
            }
            _tabControl.SelectedIndex = ind;
            CurrentListerView = ((TabItem)_tabControl.Items[ind]).Content as ListerView; ;
            if (CurrentListerView == null) return;
            SetFocus(null, null);
        }

        public void SetHeader(string header)
        {
            var currentTab = _tabControl.SelectedItem as TabItem;
            if (currentTab != null)
            {
                currentTab.Header = header;
            }
        }

        public List<ILister> Listers
        {
            get
            {
                return _tabControl.Items.Cast<TabItem>()
                    .Select(ti => ((ListerView) ti.Content).Lister)
                    .Cast<ILister>()
                    .ToList();
            }
        }

        public void SetView(string view)
        {
            var xamlView = CurrentListerView.FindResource(view) as ViewBase;
            if (xamlView == null)
            {
                throw new Exception("View not found: " + view);
            }
            CurrentListerView.View.View = xamlView;
            var itemsPanel = (ItemsPanelTemplate)CurrentListerView.FindResource(view == "fullView" ? "gridVirtualizing" : "tileVirtualizing");
            CurrentListerView.View.ItemsPanel = itemsPanel;
            EnsureFocusOnItemChange();
        }

        public void EnsureFocusOnItemChange()
        {
            CurrentListerView.View.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
        }

        void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (CurrentListerView.View.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated) return;
            
            CurrentListerView.View.ItemContainerGenerator.StatusChanged -= ItemContainerGenerator_StatusChanged;
            SetFocusToIndex(CurrentListerView.View.SelectedIndex);
        }

        private void SetFocusToIndex(int index)
        {
            index = Math.Max(index, 0);
            CurrentListerView.View.SelectedIndex = index;
            var item = CurrentListerView.View.ItemContainerGenerator.ContainerFromIndex(index) as ListViewItem;
            if (item != null)
            {
                CurrentListerView.View.ScrollIntoView(item);
                Keyboard.Focus(item);
                item.Focus();
            }
            else
            {
                EnsureFocusOnItemChange();
            }
        }

        public void ListerViewGotFocus(object sender, RoutedEventArgs e)
        {
            CurrentListerView = sender as ListerView;
            //TextInput.popup.PlacementTarget = CurrentListerView;
        }

        public void SetFocus(object sender, RoutedEventArgs args)
        {
            var listView = CurrentListerView.View;
            if (listView == null) { return; }
            SetFocusToIndex(listView.SelectedIndex);
        }

        void ListerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "CurrentDirectory":
                    ListerManager.Instance.CommandManager.SetFilter(null);
                    ListerManager.Instance.CommandManager.UpdateColumnWidth();
                    break;
                case "Filter":
                    CurrentListerView.View.SelectedIndex = 0;
                    break;
            }
        }

        private readonly TabControl _tabControl;
        public ListboxTextInput TextInput { get; private set; }
    }
}
