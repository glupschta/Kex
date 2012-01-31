using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using Kex.Common;
using Kex.Controller.PopupHandler;
using Kex.Model;
using Kex.Model.ItemProvider;
using Kex.Modell;
using Kex.Views;

namespace Kex.Controller
{
    public class CommandManager : ICommandManager
    {

        public CommandManager(ListboxTextInput listInput)
        {
            _listInput = listInput;
        }

        public ListerView CurrentView
        {
            get { return ListerManager.Instance.ListerViewManager.CurrentListerView; }
            set { ListerManager.Instance.ListerViewManager.CurrentListerView = value; }
        }

        public IItem CurrentItem {
            get { return CurrentView.View.SelectedItem as IItem; }
            set
            {
                CurrentView.View.SelectedItem = value;
                SetFocusToItem(value);
            }
        }

        public void DoDefaultAction()
        {
            try
            {
                CurrentView.Lister.ItemProvider.DoAction(CurrentItem);
            } catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        public void SetContainer(string directory)
        {
            ListerManager.Instance.ListerViewManager.SetHeader(directory);
            ListerManager.Instance.ListerViewManager.TextInput.ListItems = null;
            CurrentView.Lister.CurrentDirectory = directory;
            SetFilter(null);
            CurrentView.View.Items.SortDescriptions.Clear();
            SetFocusToIndex(0);
        }

        public void ShowSortPopup()
        {
            _listInput.Handler = new PopupSortingHandler();
            _listInput.Show();
        }

        public void ShowViewPopup()
        {
            _listInput.Handler = new PopupViewHandler();
            _listInput.Show();
        }

        public void ShowFilterPopup(bool keepText = false)
        {
            _listInput.Handler = new PopupFilterHandler();
            _listInput.Show(keepText);
        }

        public void ShowBrowsingPopup(bool keepText = false)
        {
            if (keepText)
            {
                SetFilter(null);
            }
            _listInput.Handler = new PopupBrowsingHandler(_listInput);
            _listInput.Show(keepText);
        }

        public void ShowDrivesPopup()
        {
            _listInput.Handler = new PopupDrivesHandler();
            _listInput.Show();
        }

        public void ShowSpecialFolderPopup()
        {
            _listInput.Handler = new PopupSpecialFolderHandler();
            _listInput.Show();
        }

        public void ShowEnterUrlPopup()
        {
            _listInput.Handler = new PopupEnterUrlHandler();
            _listInput.Show();
        }

        public void ShowNetWorkComputers()
        {
            _listInput.Handler = new PopupNetworkComputersHandler();
            _listInput.Show();
        }

        public void FocusView()
        {
            SetFocusToIndex(CurrentView.View.SelectedIndex);
        }

        public void SetView(string view)
        {
            CurrentView.ViewHandler.SetView(view);
            EnsureFocusOnItemChange();
        }

        public void EnsureFocusOnItemChange()
        {
            CurrentView.View.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
        }

        void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (_listInput.popup.IsOpen
                || CurrentView.View.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated
            ) return;
            
            CurrentView.View.ItemContainerGenerator.StatusChanged -= ItemContainerGenerator_StatusChanged;
            var index = CurrentView.View.SelectedIndex;
            SetFocusToIndex(Math.Max(index,0));
        }

        public void SetSorting(string selectedColumn)
        {
            var currentSorting = CurrentView.View.Items.SortDescriptions.FirstOrDefault();
            var direction = ListSortDirection.Ascending;
            if (currentSorting.PropertyName != null)
            {
                if (currentSorting.PropertyName.Equals(selectedColumn, StringComparison.OrdinalIgnoreCase))
                direction = currentSorting.Direction == ListSortDirection.Ascending
                                ? ListSortDirection.Descending
                                : ListSortDirection.Ascending;
            }
            var selectedItem = CurrentItem;
            ClearSorting();
            
            SetFocusToItem(selectedItem);
            CurrentView.View.Items.SortDescriptions.Add(new SortDescription(selectedColumn, direction));
        }

        public void ClearSorting()
        {
            CurrentView.View.Items.SortDescriptions.Clear();
        }

        public void GroupByName()
        {
            var groupDescription = new PropertyGroupDescription("Name", new FileExtensionValueConverter());
            CurrentView.View.Items.GroupDescriptions.Add(groupDescription);
            var items = CurrentView.View.Items;
            var firstItem = items.Count == 0 ? null : items[0] as IItem;
            if (firstItem != null)
            {
                SetFocusToItem(firstItem);
                FocusView();
            }
        }

        internal class FileExtensionValueConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                var val = value as string;
                var dotIndex = val.LastIndexOf(".");
                if (dotIndex == -1)
                    return "Directory";
                else
                {
                    return val.Substring(dotIndex);
                }
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        public void SetFilter(string filter)
        {
            if (filter == null && CurrentView.View.Items.Filter == null)
                return;
            if (filter == null)
            {
                CurrentView.Lister.Filter = null;
                CurrentView.View.Items.Filter = null;
                EnsureFocusOnItemChange();
            }
            else
            {
                CurrentView.Lister.Filter = filter;
                CurrentView.View.Items.Filter = delegate(object item) { return ((IItem) item).Name.ToLower().Contains(filter.ToLower()); };
            }
        }

        public void ClosePopup()
        {
            _listInput.Close();
        }

        public void UpdateColumnWidth()
        {
            var gridView = CurrentView.View.View as GridView;
            if (gridView == null) return;
            foreach(var column in gridView.Columns.Where(co => Double.IsNaN(co.Width)))
            {
                column.Width = column.ActualWidth;
                column.Width = double.NaN;
            }
        }

        public void ShowShellPropertyPopup()
        {
            _listInput.Handler = new ShellPropertyPopupHandler();
            _listInput.Show();
        }

        public void ClearGrouping()
        {
            if (CurrentView.View.Items.GroupDescriptions != null)
                CurrentView.View.Items.GroupDescriptions.Clear();
        }

        public void Select(IItem item)
        {
            CurrentView.View.SelectedItem = item;
            SetFocusToItem(item);
        }

        public void GoUp()
        {
            DoTraverse(FocusNavigationDirection.Up);
        }

        public void GoDown()
        {
            DoTraverse(FocusNavigationDirection.Down);
        }

        public void GoLeft()
        {
            DoTraverse(FocusNavigationDirection.Left);
        }

        public void GoRight()
        {
            DoTraverse(FocusNavigationDirection.Right);
        }

        private static void DoTraverse(FocusNavigationDirection direction)
        {
            var uie = Keyboard.FocusedElement as UIElement;
            if (uie == null) return;
            uie.MoveFocus(new TraversalRequest(direction)); 
        }

        public void HistoryBack()
        {
            var historyItem = CurrentView.Lister.HistoryBack();
            SetContainer(historyItem.FullPath);
            var selected = CurrentView.Lister.Items.FirstOrDefault(i => i.FullPath == historyItem.SelectedPath);
            SetFocusToItem(selected);
        }

        public void HistoryForward()
        {
            var historyItem = CurrentView.Lister.HistoryForward();
            SetContainer(historyItem.FullPath);
        }

        public void ContainerUp()
        {
            var dup = CurrentView.Lister.ContainerUp();
            if (dup == null) return;
            SetContainer(dup);
        }

        public void CycleListers(int direction)
        {
            var ind = _views.IndexOf(CurrentView);
            ind += direction;
            if (ind < 0)
            {
                ind = _views.Count -1;
            }
            else if (ind > _views.Count - 1)
            {
                ind = 0;
            }
            CurrentView = _views[ind];
            if (CurrentView == null) return;
            FocusView();
        }

        public void FitWidthToListers()
        {
            UpdateColumnWidth();
            var grid = CurrentView.View.View as GridView;
            if (grid != null)
            {
                var colWidth = grid.Columns.Sum(c => c.ActualWidth);
                Application.Current.MainWindow.Width = colWidth + 80;
            }
        }

        public void ShowListers()
        {
            
        }

        public void ShowLocationInput()
        {
            
        }

        public void HandleException(Exception ex)
        {
            UpdateColumnWidth();
            MessageBox.Show(ex.ToString(), "Error");
        }

        public void SelectAll()
        {
            CurrentView.View.SelectAll();
        }

        public void ClearSelection()
        {
            CurrentView.View.UnselectAll();
        }

        public void Copy()
        {
            FileAction.SetSelection();
            FileAction.ActionType = FileActionType.Copy;
        }

        public void Cut()
        {
            FileAction.SetSelection();
            FileAction.ActionType = FileActionType.Move;
        }

        public void Paste()
        {
            string lastPath = FileAction.Paste(CurrentView.Lister.CurrentDirectory);
            CurrentView.Lister.Refresh();
            CurrentItem = CurrentView.Lister.Items.SingleOrDefault(i => i.FullPath == lastPath);
        }

        public void Delete()
        {
            var list = CurrentView.Lister.Items.ToList();
            int minIndex = list.Min(item =>list.IndexOf(item));
            var focusedAfterDelete = list[Math.Max(0, minIndex - 1)];
            FileAction.Delete();
            CurrentView.Lister.Refresh();
            CurrentItem = focusedAfterDelete;
        }

        public void ShowContextMenu()
        {
            var sel = CurrentItem;
            var position = GetCurrentItemPosition();
            if (position == null)
            {
                return;
            }
            var menu = new ShellContextMenu1();
            if (sel.ItemType == ItemType.Container)
            {
                var dir = new[] { new DirectoryInfo(sel.FullPath) };
                menu.ShowContextMenu(dir, position.Value);
            }
            else
            {
                var file = new[] { new FileInfo(sel.FullPath) };
                menu.ShowContextMenu(file, position.Value);
            }
        }

        private System.Drawing.Point? GetCurrentItemPosition()
        {
            var sel = CurrentItem;
            var listItem = CurrentView.View.ItemContainerGenerator.ContainerFromItem(sel) as ListViewItem;
            if (listItem == null)
                return null;
            var window = Application.Current.MainWindow;
            var t = listItem.TransformToAncestor(window).Transform(new Point(0, 0));
            var p = window.PointToScreen(new Point(t.X, t.Y));
            var x = (int)p.X + 10;
            var y = (int)p.Y + 10;
            return new System.Drawing.Point(x, y);
        }

        public void ShowFavorites()
        {
            var favoriteLocation = Environment.GetFolderPath(Environment.SpecialFolder.Favorites);
            SetContainer(favoriteLocation);
        }

        public void SetFocusToItem(IItem iitem)
        {
            var item = CurrentView.View.ItemContainerGenerator.ContainerFromItem(iitem) as ListViewItem;
            if (item != null)
            {
                Keyboard.Focus(item);
                item.Focus();
                CurrentView.View.ScrollIntoView(item);
            }
            else
            {
                EnsureFocusOnItemChange();
            }
        }

        private void SetFocusToIndex(int index)
        {
            if (index < 0)
                index = 0;
            CurrentView.View.SelectedIndex = index;
            var item = CurrentView.View.ItemContainerGenerator.ContainerFromIndex(index) as ListViewItem;
            if (item != null)
            {
                Keyboard.Focus(item);
                item.Focus();
                CurrentView.View.ScrollIntoView(item);
            } 
            else
            {
                EnsureFocusOnItemChange();
            }
        }

        internal readonly ListboxTextInput _listInput;
        private readonly List<ListerView> _views = new List<ListerView>();
    }
}
