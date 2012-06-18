using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using Kex.Common;
using Kex.Controller.Popups;
using Kex.Model;
using Kex.Model;
using Kex.Model.ItemProvider;
using Kex.Views;
using Microsoft.WindowsAPICodePack.Shell;
using Application = System.Windows.Application;
using ListViewItem = System.Windows.Controls.ListViewItem;
using MessageBox = System.Windows.MessageBox;

namespace Kex.Controller
{
    public class CommandManager
    {
        public CommandManager(IPopupInput popupInput, IListerViewManager viewManager)
        {
            _PopupInput = popupInput;
            _ListerViewManager = viewManager;
        }

        public ListerView CurrentView
        {
            get { return _ListerViewManager.CurrentListerView; }
            set { _ListerViewManager.CurrentListerView = value; }
        }

        public IItem CurrentItem
        {
            get
            {
                return CurrentView.View.SelectedItem as IItem;
            }
            set
            {
                CurrentView.View.SelectedItem = value;
                SetFocusToItem(value);
            }
        }

        public void ExecuteByCommandName(string commandName, string argument = null)
        {
            switch (commandName.ToLower())
            {
                case "windowadjustsize":
                    WindowAdjustSize();
                    break;
                case "windowmaximize":
                    WindowMaximize();
                    break;
                case "windowminimize":
                    WindowMinimize();
                    break;
                case "windowrestore":
                    WindowRestore();
                    break;
                case "windowdockleft":
                    WindowDockLeft();
                    break;
                case "windowdockright":
                    WindowDockRight();
                    break;
                case "windowclose":
                    _ListerViewManager.CloseCurrentLister();
                    break;
                case "savetabs":
                    SaveTabs();
                    break;
                case "clearreadonly":
                    ClearReadonly();
                    break;
                case "executeexternal":
                    ExecuteExternal(argument);
                    break;
                case "sendmail":
                    SendWithMail();
                    break;
                case "addtofavorites":
                    AddToFavorites();
                    break;
            }
        }

        private void AddToFavorites()
        {
            var current = CurrentItem;
            var favoriteLocation = Environment.GetFolderPath(Environment.SpecialFolder.Favorites);
            var linkPath = Path.Combine(favoriteLocation, current.Name + ".lnk");
            var targetPath = current.FullPath;

            if (current.ItemType == ItemType.Container)
            {
                   SymbolicLink.CreateDirectoryLink(linkPath, targetPath);
            }
            else
            {
                 SymbolicLink.CreateFileLink(linkPath, targetPath);
            }
        }

        private void SendWithMail()
        {
            var selectedfiles = ListerManager.Instance.ListerViewManager.CurrentListerView
                .GetSelection().Select(s => s.FullPath);

            var message = new MapiMailMessage("", "");
            foreach (var file in selectedfiles)
            {
                message.Files.Add(file);
            }
            message.ShowDialog();
        }

        private void ExecuteExternal(string executableLocation)
        {
            var selectedfiles = ListerManager.Instance.ListerViewManager.CurrentListerView
                .View.SelectedItems.Cast<IItem>().Select(s => "\""+ s.FullPath+"\"");
            var psi = new ProcessStartInfo(executableLocation);
            psi.Arguments =string.Join(" ", selectedfiles);
            Process.Start(psi);
        }

        public void SaveTabs()
        {
            var diag = new SaveFileDialog();
            if (diag.ShowDialog() == DialogResult.OK)
            {
                SaveTabs(diag.FileName);
            }
        }

        public void SaveTabs(string filename)
        {
            File.WriteAllLines(filename, _ListerViewManager.Listers.Select(li => li.CurrentDirectory));
        }

        public void WindowAdjustSize()
        {
            FitWidthToListers();
            FitWidthToListers();
        }

        public void WindowMaximize()
        {
            Application.Current.MainWindow.WindowState = WindowState.Maximized;
        }

        public void WindowMinimize()
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
            
        }

        public void WindowRestore()
        {
            Application.Current.MainWindow.WindowState = WindowState.Normal;
        }

        public void WindowDockLeft()
        {
            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;
            Application.Current.MainWindow.Top = 0;
            Application.Current.MainWindow.Left = 0;
            Application.Current.MainWindow.Width = screenWidth/2;
            Application.Current.MainWindow.Height = screenHeight;
        }

        public void WindowDockRight()
        {
            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;
            Application.Current.MainWindow.Top = 0;
            Application.Current.MainWindow.Left = screenWidth-screenWidth/2;
            Application.Current.MainWindow.Width = screenWidth -Application.Current.MainWindow.Left;
            Application.Current.MainWindow.Height = screenHeight;            
        }


        public void DoDefaultAction()
        {
            DoDefaultAction(CurrentItem);
        }

        public void DoDefaultAction(IItem item)
        {
            try
            {
                if (item.FullPath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                {
                    var zipItemProvider = new ZipItemProvider(item.FullPath);
                    var lister = new ZipLister(CurrentView.Lister) {ItemProvider = zipItemProvider};
                    CurrentView.Lister = lister;
                    CurrentView.DataContext = lister;
                }
                else
                {
                    CurrentView.Lister.ItemProvider.DoAction(item);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            } 
        }

        public void SetContainer(string directory)
        {
            _ListerViewManager.SetHeader(directory);
            CurrentView.Lister.CurrentDirectory = directory;
            SetFilter(null);
            CurrentView.View.Items.SortDescriptions.Clear();
            CurrentView.View.SelectedIndex = 0;
            UpdateColumnWidth();
            FocusView();
        }

        public void Rename()
        {
            var view = ListerManager.Instance.ListerViewManager.CurrentListerView;
            view.renamePopup.Show(view);
        }

        public void ShowSortPopup()
        {
            new SortPopup(_PopupInput).Show();
        }

        public void ShowViewPopup()
        {
            new ViewPopup(_PopupInput).Show();
        }

        public void ShowFilterPopup(bool keepText = false)
        {
            new FilterPopup(_PopupInput).Show();
        }

        public void ShowBrowsingPopup(bool keepText = false)
        {
            new BrowsingPopup(_PopupInput).Show();
        }

        public void ShowDrivesPopup()
        {
            new DrivesPopup(_PopupInput).Show();
        }

        public void ShowNetWorkComputers()
        {
            new NetworkComputersPopup(_PopupInput).Show();
        }

        public void ShowMenu()
        {
            new MenuPopup(_PopupInput, "MainMenu").Show();
        }

        public void ShowOpenLocationPopup(bool newWindow)
        {
            var locationPopup = new OpenLocationPopup(_PopupInput, newWindow);
            locationPopup.Show();
        }

        public void ShowCreateFileDirectoryPopup()
        {
            new CreateFileDirectoryPopup(_PopupInput).Show();
        }

        public void FocusView()
        {
            CurrentItem = CurrentView.View.SelectedItem as IItem;
        }

        public void SetView(string view)
        {
            selectedBeforeViewChange = CurrentItem;
            CurrentView.ViewHandler.SetView(view);
            CurrentView.View.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
        }

        private IItem selectedBeforeViewChange;

        void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (_PopupInput.IsOpen 
                || CurrentView.View.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                return;
            
            CurrentView.View.ItemContainerGenerator.StatusChanged -= ItemContainerGenerator_StatusChanged;
            var lastSelection = selectedBeforeViewChange != null ? selectedBeforeViewChange.FullPath : null;
            CurrentItem = CurrentView.View.Items.OfType<IItem>().First(i => i.FullPath == lastSelection);
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
            CurrentView.View.Items.SortDescriptions.Add(new SortDescription(selectedColumn, direction));
            CurrentItem = selectedItem;
        }

        public void ClearSorting()
        {
            CurrentView.View.Items.SortDescriptions.Clear();
            FocusView();
        }

        public void GroupByName()
        {
            var groupDescription = new PropertyGroupDescription("Name", new FileExtensionValueConverter());
            CurrentView.View.Items.GroupDescriptions.Add(groupDescription);
            var items = CurrentView.View.Items;
            var firstItem = items.Count == 0 ? null : items[0] as IItem;
            if (firstItem != null)
            {
                CurrentItem = firstItem;
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
                FocusView();
            }
            else
            {
                CurrentView.Lister.Filter = filter;
                CurrentView.View.Items.Filter = delegate(object item) { return ((IItem) item).Name.ToLower().Contains(filter.ToLower()); };
            }
        }

        public void ClosePopup()
        {
            _PopupInput.Hide();
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

        public void ClearGrouping()
        {
            if (CurrentView.View.Items.GroupDescriptions != null)
                CurrentView.View.Items.GroupDescriptions.Clear();
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
            if (historyItem.FullPath != CurrentView.Lister.CurrentDirectory)
            {
                if (EnsureListerType(historyItem))
                {
                    SetContainer(historyItem.FullPath);
                    CurrentItem = CurrentView.Lister.Items.FirstOrDefault(i => i.FullPath == historyItem.SelectedPath);
                }
            }
        }

        public void HistoryForward()
        {
            var historyItem = CurrentView.Lister.HistoryForward();
            if (EnsureListerType(historyItem))
                SetContainer(historyItem.FullPath);
        }

        private bool EnsureListerType(HistoryItem historyItem)
        {
            if (historyItem == null) return false;
            if (historyItem.ListerType != CurrentView.Lister.GetType())
            {
                var baseLister = CurrentView.Lister;
                var lister = historyItem.ListerType
                    .GetConstructor(new [] { typeof(ILister), typeof(string) })
                    .Invoke(new object[] {baseLister, historyItem.FullPath });
                CurrentView.Lister = lister as ILister<FileProperties>;
                return false;
            }
            return true;
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

                var selection = ListerManager.Instance.ListerViewManager.CurrentListerView.View.SelectedItems;
                if (selection.Count == 0) return;
                var ask = new UserChoicePopup(_PopupInput, "Delete?", new[] {"Yes", "No", "Dont Know"});
                ask.SelectionDone += ask_SelectionDone;
                ask.Show();
        }

        void ask_SelectionDone(object sender, string selectedAnswer)
        {
            if (selectedAnswer == "Yes")
            {
                var selection = ListerManager.Instance.ListerViewManager.CurrentListerView.View.SelectedItems;
                var itemContainer = CurrentView.View.ItemContainerGenerator;
                var container = itemContainer.ContainerFromItem(selection);
                var index = (container != null) ? itemContainer.IndexFromContainer(container) - 1 : 0;

                var focusedContainer = itemContainer.ContainerFromIndex(Math.Max(0, index));
                var focusedItem = itemContainer.ItemFromContainer(focusedContainer) as IItem;
                var focusedPath = focusedItem.FullPath;
                FileAction.Delete();
                CurrentView.Lister.Refresh();
                CurrentItem = CurrentView.Lister.Items.FirstOrDefault(i => i.FullPath == focusedPath)
                              ?? CurrentView.Lister.Items.FirstOrDefault();
            }
        }

        public void ShowProperties()
        {
            var dialog = new PropertyDialog();
            dialog.Show(CurrentItem as FileItem);
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

        public void ClearReadonly()
        {
            var path = CurrentItem.FullPath;
            var attr = File.GetAttributes(CurrentItem.FullPath);

            if ((attr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                File.SetAttributes(path, (attr ^ FileAttributes.ReadOnly));
        }

        private void SetFocusToItem(IItem iitem)
        {
            var listViewItem = CurrentView.View.ItemContainerGenerator.ContainerFromItem(iitem) as ListViewItem;
            if (listViewItem == null)
            {
                CurrentView.View.ScrollIntoView(iitem);
                listViewItem = CurrentView.View.ItemContainerGenerator.ContainerFromItem(iitem) as ListViewItem;
            }
            if (listViewItem != null)
            {
                listViewItem.Focus(); 
                Keyboard.Focus(listViewItem);
            }
        }

        internal readonly IPopupInput _PopupInput;
        private readonly IListerViewManager _ListerViewManager;
        private readonly List<ListerView> _views = new List<ListerView>();
    }
}
