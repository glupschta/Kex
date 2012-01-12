using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Kex.Common;
using Kex.Model.ItemProvider;
using Kex.Modell;
using Kex.Views;

namespace Kex.Controller
{
    public class TabbedListerViewManager : IListerViewManager
    {
        public TabbedListerViewManager(TabControl tabControl, ListboxTextInput textInput)
        {
            TabControl = tabControl;
            TextInput = textInput;
        }

        public ListerView CurrentListerView { get; set; }

        public void OpenLister(string directory)
        {
            var itemProvider = new FilesystemItemProvider(directory);
            var lister = new FileLister { ItemProvider = itemProvider };
            var listerView = new ListerView { Lister = lister, DataContext = lister };

            var newTab = new TabItem();
            newTab.Header = directory;
            newTab.Content = listerView;
            TabControl.Items.Add(newTab);

            CurrentListerView = listerView;
            SetView("fullView");

            listerView.GotFocus += ListerViewGotFocus;
            listerView.View.Loaded += SetFocus;
            listerView.Lister.PropertyChanged += ListerPropertyChanged;
            TabControl.SelectedItem = newTab;
        }

        void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.OriginalSource is TabControl)
            {
                var tab = TabControl.Items[TabControl.SelectedIndex] as TabItem;
                CurrentListerView = tab.Content as ListerView;
                e.Handled = true;
            }
        }


        public void CloseCurrentLister()
        {
            if (TabControl.Items.Count == 1) return;
            var ind = TabControl.SelectedIndex;
            TabControl.Items.Remove(TabControl.SelectedItem);
            if (ind > TabControl.Items.Count -1)
            {
                ind = TabControl.Items.Count - 1;
            }
            CurrentListerView = ((TabItem) TabControl.Items[ind]).Content as ListerView;
            SetFocus(null, null);
        }

        public void CycleListers(int direction)
        {
            var ind = TabControl.SelectedIndex;
            ind += direction;
            if (ind < 0)
            {
                ind = TabControl.Items.Count - 1;
            }
            else if (ind > TabControl.Items.Count - 1)
            {
                ind = 0;
            }
            TabControl.SelectedIndex = ind;
            CurrentListerView = ((TabItem)TabControl.Items[ind]).Content as ListerView; ;
            if (CurrentListerView == null) return;
            SetFocus(null, null);
        }

        public void SetHeader(string header)
        {
            var currentTab = TabControl.SelectedItem as TabItem;
            if (currentTab != null)
            {
                currentTab.Header = header;
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
            if (CurrentListerView.View.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                CurrentListerView.View.ItemContainerGenerator.StatusChanged -= ItemContainerGenerator_StatusChanged;
                var index = CurrentListerView.View.SelectedIndex;
                if (index < 0) index = 0;
                SetFocusToIndex(index);
            }
        }

        private void SetFocusToIndex(int index)
        {
            if (index < 0)
                index = 0;
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
            TextInput.popup.PlacementTarget = CurrentListerView;
        }

        public void SetFocus(object sender, RoutedEventArgs args)
        {
            var listView = CurrentListerView.View;
            if (listView == null) { return; }
            if (listView.SelectedIndex < 0)
            {
                listView.SelectedIndex = 0;
            }
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

        private readonly TabControl TabControl;
        public ListboxTextInput TextInput { get; private set; }
    }
}
