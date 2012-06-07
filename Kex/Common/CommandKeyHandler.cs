using System.Collections.Generic;
using System.Windows.Input;
using Kex.Controller;
using Kex.Model;

namespace Kex.Common
{
    public class CommandKeyHandler
    {
        public static void HandleKey(KeyEventArgs e)
        {
            HandleKey(e, false);
        }

        public static void HandleKey(KeyEventArgs e, bool ignoreCtrl)
        {
            var ignoredKeys = new List<Key> { Key.LeftAlt, Key.LeftCtrl, Key.LeftCtrl, Key.RightAlt, Key.RightCtrl, Key.RightShift };
            if (ignoredKeys.Contains(e.Key)) return;
            bool shift = ((e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift);
            bool control = !ignoreCtrl && ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control);
            bool alt = ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt);

            var k = e.Key;
            switch(k)
            { 
                case Key.Delete:
                    ListerManager.Instance.CommandManager.Delete();
                    break;
                case Key.Escape:
                    ListerManager.Instance.CommandManager.ClosePopup();
                    break;
                case Key.H:
                    if (shift)
                        ListerManager.Instance.CommandManager.ContainerUp();
                    else
                        ListerManager.Instance.CommandManager.HistoryBack();
                    break;
                case Key.A:
                    if (shift)
                        ListerManager.Instance.CommandManager.ClearSelection();
                    else
                        ListerManager.Instance.CommandManager.SelectAll();
                    break;
                case Key.B:
                    ListerManager.Instance.CommandManager.ShowShellPropertyPopup();
                    break;
                case Key.C:
                    if (control)
                        ListerManager.Instance.CommandManager.Copy();
                    break;
                case Key.X:
                    if (control)
                        ListerManager.Instance.CommandManager.Cut();
                    break;
                case Key.V:
                    if (control)
                        ListerManager.Instance.CommandManager.Paste();
                    break;
                case Key.Return:
                    ListerManager.Instance.CommandManager.DoDefaultAction();
                    break;
                case Key.O:
                    ListerManager.Instance.CommandManager.ShowEnterUrlPopup();
                    break;
                case Key.I:
                    ListerManager.Instance.CommandManager.ShowBrowsingPopup(ignoreCtrl);
                    break;
                case Key.L:
                    ListerManager.Instance.CommandManager.HistoryForward();
                    break;
                case Key.M:
                    ListerManager.Instance.CommandManager.ShowMenu();
                    break;
                case Key.N:
                    ListerManager.Instance.CommandManager.ShowViewPopup();
                    break;
                case Key.J:
                        ListerManager.Instance.CommandManager.GoDown();
                    break;
                case Key.K:
                        ListerManager.Instance.CommandManager.GoUp();
                    break;
                case Key.D:
                    if (shift)
                        ListerManager.Instance.CommandManager.ShowNetWorkComputers();
                    else
                        ListerManager.Instance.CommandManager.ShowDrivesPopup();
                    break;
                case Key.Q:
                        ListerManager.Instance.CommandManager.FitWidthToListers();
                    break;
                case Key.E:
                    WindowsFunctions.ShowFileProperties(ListerManager.Instance.CommandManager.CurrentItem.FullPath);
                    break;
                case Key.F:
                    ListerManager.Instance.CommandManager.ShowFavorites();
                    break;
                case Key.G:
                    if (shift)
                        ListerManager.Instance.CommandManager.ClearGrouping();
                    else
                        ListerManager.Instance.CommandManager.GroupByName();
                    break;

                case Key.P:
                    ListerManager.Instance.ListerViewManager.CloseCurrentLister();
                    break;
                case Key.R:
                    ListerManager.Instance.CommandManager.ShowContextMenu();
                    break;
                case Key.S:
                    if (shift)
                        ListerManager.Instance.CommandManager.ClearSorting();
                    else
                        ListerManager.Instance.CommandManager.ShowSortPopup();
                    break;
                case Key.T:
                    ListerManager.Instance.CommandManager.Rename();
                    break;
                case Key.W:
                    if (shift)
                        ListerManager.Instance.CommandManager.SetFilter(null);
                    else
                        ListerManager.Instance.CommandManager.ShowFilterPopup(ignoreCtrl);
                    break;

                case Key.Tab:
                    if (shift)
                        ListerManager.Instance.ListerViewManager.CycleListers(-1);
                    else
                        ListerManager.Instance.ListerViewManager.CycleListers(1);
                    break;
                    break;
                case Key.Oem3:
                    ListerManager.Instance.ListerViewManager.OpenLister(ListerManager.Instance.CommandManager.CurrentItem.FullPath);
                    break;
                case Key.Back:
                    ListerManager.Instance.CommandManager.ContainerUp();
                    break;
                default:
                    e.Handled = false;
                    break;
            }
            e.Handled = true;
        }

    }
}
