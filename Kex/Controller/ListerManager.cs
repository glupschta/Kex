using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Kex.Interfaces;
using Kex.ItemProvider;
using Kex.PopupHandler;
using Kex.Views;

namespace Kex.Controller
{

    public class ListerManager : IListerViewHandler
    {
        private readonly TextBlock _header;
        public static void Initialize(TextBlock header, Panel container, ListboxTextInput listInput)
        {
            Manager = new ListerManager(header, container,  listInput);
        }

        private ListerManager(TextBlock header, Panel container, ListboxTextInput listInput)
        {
            _container = container;
            _listInput = listInput;
            _header = header;
        }

        public static ListerManager Manager { get; private set; }

        public ListerView CurrentView { get; set; }

        public ILister CurrentLister
        {
            get { return CurrentView.Lister; }
        }

        public IItem CurrentItem {
            get { return CurrentView.View.SelectedItem as IItem; }
            set { CurrentView.View.SelectedItem = value; }
        }

        public ItemSelection CurrentSelection { get; set; }

        private ItemSelection GetSelectedItems()
        {
            var selection = CurrentView.Lister.SelectedItems ?? new ItemSelection
                                                                    {
                                                                        Selection = new List<IItem> {CurrentItem}
                                                                    };
            return selection;
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

        public void SetDirectory(string directory)
        {
            CurrentView.Lister.ItemProvider = new FilesystemItemProvider(directory);
            CurrentView.Lister.CurrentDirectory = directory;
            CurrentView.View.Items.Filter = null;
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

        public void ShowFilterPopup()
        {
            _listInput.Handler = new PopupFilterHandler();
            _listInput.Show();
        }

        public void ShowBrowsingPopup()
        {
            _listInput.Handler = new PopupBrowsingHandler(_listInput);
            _listInput.Show();
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

        public void FocusView()
        {
            SetFocusToIndex(CurrentView.View.SelectedIndex);
        }

        public void SetView(string view)
        {
            var xamlView = CurrentView.FindResource(view) as ViewBase;
            EnsureFocusOnItemChange();
            CurrentView.View.View = xamlView;
        }

        public void EnsureFocusOnItemChange()
        {
            CurrentView.View.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
        }

        void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (_listInput.popup.IsOpen) return;
            if (CurrentView.View.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                CurrentView.View.ItemContainerGenerator.StatusChanged -= ItemContainerGenerator_StatusChanged;
                var index = CurrentView.View.SelectedIndex;
                if (index < 0) index = 0;
                SetFocusToIndex(index);
            }
        }


        public void SetSorting(string selectedColumn, bool descending)
        {
            CurrentView.View.Items.SortDescriptions.Clear();
            var sortDescription = new SortDescription(selectedColumn,
                                                      descending
                                                          ? ListSortDirection.Descending
                                                          : ListSortDirection.Ascending);
            CurrentView.View.Items.SortDescriptions.Add(sortDescription);
        }

        public void SetFilter(string filter)
        {
            if (filter == null && CurrentView.View.Items.Filter == null)
                return;
            if (filter == null)
            {
                CurrentView.View.Items.Filter = null;
                EnsureFocusOnItemChange();
            }
            else
                CurrentView.View.Items.Filter = delegate(object item) { return ((IItem)item).Name.ToLower().Contains(filter.ToLower()); };
        }

        public void ClosePopup()
        {
            _listInput.Close();
        }

        private void UpdateColumnWidth()
        {
            var gridView = CurrentView.View.View as GridView;
            if (gridView == null) return;
            foreach(var column in gridView.Columns.Where(co => Double.IsNaN(co.Width)))
            {
                column.Width = column.ActualWidth;
                column.Width = double.NaN;
            }
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
            SetDirectory(CurrentView.Lister.HistoryBack());
        }

        public void HistoryForward()
        {
            SetDirectory(CurrentView.Lister.HistoryForward());
        }

        public void DirectoryUp()
        {
            var dup = CurrentView.Lister.DirectoryUp();
            if (dup == null) return;
            SetDirectory(dup);
        }

        public void OpenLister()
        {
            OpenLister(CurrentItem.ResolvedPath);
        }

        public void OpenLister(string directory)
        {
            var itemProvider = new FilesystemItemProvider(directory);
            var lister = new Lister {ItemProvider = itemProvider};
            var listerView = new ListerView { Lister = lister, DataContext = lister };
            var ind = _views.Count > 0 ? _views.IndexOf(CurrentView) + 1 : 0;
            _views.Insert(ind, listerView);
            _container.Children.Insert(ind, listerView);
            CurrentView = listerView;
            listerView.GotFocus += ListerViewGotFocus;
            listerView.View.Loaded += SetFocus;
            listerView.Lister.PropertyChanged += ListerPropertyChanged;
        }

        void ListerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "CurrentDirectory":
                    SetFilter(null);
                    UpdateColumnWidth();
                    break;
                case "Filter":
                    CurrentView.View.SelectedIndex = 0;
                    break;
            }
        }

        public void CloseNotFocusedListers()
        {
            var viewNotCurrent = _views
                .Where(v => v != CurrentView).ToArray();
            foreach (var view in viewNotCurrent)
            {
                _container.Children.Remove(view);
                _views.Remove(view);
            }
        }

        public void CloseCurrentLister()
        {
            if (_views.Count <= 1) return;
            var ind = _views.IndexOf(CurrentView);
            _views.Remove(CurrentView);
            _container.Children.Remove(CurrentView);
            if (ind > _views.Count - 1)
            {
                ind = _views.Count - 1;
            }
            CurrentView = _views[ind];
            SetFocus(null, null);
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
            SetFocus(null, null);
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
            foreach(var it in CurrentView.Lister.Items)
            {
                it.IsSelected = !it.IsSelected;
            }
            CurrentView.Lister.OnPropertyChanged("SelectedItems");
        }

        public void ClearSelection()
        {
            foreach (var it in CurrentView.Lister.Items.Where(it => it.IsSelected))
            {
                it.IsSelected = false;
            }
            CurrentView.Lister.OnPropertyChanged("SelectedItems");
        }

        public void MarkSelected()
        {
            CurrentItem.IsSelected = !CurrentItem.IsSelected;
            CurrentView.Lister.OnPropertyChanged("SelectedItems");
        }

        public void Copy()
        {
            this.CurrentSelection = GetSelectedItems();
            this.CurrentSelection.FileAction = FileActionType.Copy;
        }

        public void Cut()
        {
            this.CurrentSelection = GetSelectedItems();
            this.CurrentSelection.FileAction = FileActionType.Move;
        }

        public void Paste()
        {
            string lastPath = FileAction.Do(this.CurrentSelection, CurrentView.Lister.CurrentDirectory);
            CurrentView.Lister.Refresh();
            CurrentItem = CurrentView.Lister.Items.SingleOrDefault(i => i.FullPath == lastPath);
        }

        public void Delete()
        {
            var selected = GetSelectedItems();
            var list = CurrentView.Lister.Items.ToList();
            int minIndex = selected.Selection.Min(item =>list.IndexOf(item));
            var focusedAfterDelete = list[Math.Max(0, minIndex - 1)];
            FileAction.Delete(GetSelectedItems());
            CurrentView.Lister.Refresh();
            CurrentView.View.SelectedItem = focusedAfterDelete;
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
            if (sel.Type == ItemType.Container)
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
            SetDirectory(favoriteLocation);
        }

        public void SetFocus(object sender, RoutedEventArgs args)
        {
            var listView = CurrentView.View;
            if (listView == null){return;}
            if (listView.SelectedIndex < 0)
            {
                listView.SelectedIndex = 0;
            }
            SetFocusToIndex(listView.SelectedIndex);
        }

        private void SetFocusToItem(IItem iitem)
        {
            var item = CurrentView.View.ItemContainerGenerator.ContainerFromItem(iitem) as ListViewItem;
            if (item != null)
            {
                Keyboard.Focus(item);
                item.Focus();
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
                CurrentView.View.ScrollIntoView(CurrentView.View.SelectedItem);
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
            CurrentView = sender as ListerView;
            _listInput.popup.PlacementTarget = CurrentView;
            _listInput.popup.Placement = PlacementMode.Center;
        }

        private readonly Panel _container;
        internal readonly ListboxTextInput _listInput;
        private readonly List<ListerView> _views = new List<ListerView>();

    }
}
