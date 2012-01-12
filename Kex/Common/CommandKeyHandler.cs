using System.Windows.Input;
using Kex.Controller;
using Kex.Model;

namespace Kex.Common
{
    public class CommandKeyHandler
    {
        public static bool HandleKey(Key k, bool shift, bool control, bool alt)
        {
            switch(k)
            {
                case Key.Space:
                    ListerManager.Instance.CommandManager.MarkSelected();
                    break;
                case Key.Delete:
                    ListerManager.Instance.CommandManager.Delete();
                    break;
                case Key.Escape:
                    ListerManager.Instance.CommandManager.ClosePopup();
                    break;
                case Key.U:
                    if (shift)
                        ListerManager.Instance.CommandManager.HistoryBack();
                    else
                        ListerManager.Instance.CommandManager.DirectoryUp();
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
                    ListerManager.Instance.CommandManager.Copy();
                    break;
                case Key.X:
                    ListerManager.Instance.CommandManager.Cut();
                    break;
                case Key.V:
                    ListerManager.Instance.CommandManager.Paste();
                    break;
                case Key.Return:
                    ListerManager.Instance.CommandManager.DoDefaultAction();
                    break;
                case Key.O:
                    ListerManager.Instance.CommandManager.ShowEnterUrlPopup();
                    break;
                case Key.I:
                    ListerManager.Instance.CommandManager.ShowBrowsingPopup();
                    break;
                case Key.H:
                    ListerManager.Instance.CommandManager.GoLeft();
                    break;
                case Key.L:
                    ListerManager.Instance.CommandManager.GoRight();
                    break;
                case Key.J:
                    if (shift)
                    {
                        ListerManager.Instance.CommandManager.MarkSelected();
                        ListerManager.Instance.CommandManager.GoDown();
                    }
                    else
                        ListerManager.Instance.CommandManager.GoDown();
                    break;
                case Key.K:
                    if (shift)
                    {
                        ListerManager.Instance.CommandManager.MarkSelected();
                        ListerManager.Instance.CommandManager.GoUp();
                    }
                    else
                        ListerManager.Instance.CommandManager.GoUp();
                    break;
                case Key.D:
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
                case Key.M:
                    ListerManager.Instance.CommandManager.ShowViewPopup();
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
                case Key.Z:
                    if (shift)
                        ListerManager.Instance.CommandManager.SetFilter(null);
                    else
                        ListerManager.Instance.CommandManager.ShowFilterPopup();
                    break;
                case Key.Tab:
                    if (shift)
                        ListerManager.Instance.ListerViewManager.CycleListers(-1);
                    else
                        ListerManager.Instance.ListerViewManager.CycleListers(1);
                    break;
                case Key.W:
                    ListerManager.Instance.CommandManager.ShowListers();
                    break;
                case Key.Oem3:
                    ListerManager.Instance.ListerViewManager.OpenLister(ListerManager.Instance.CommandManager.CurrentItem.FullPath);
                    break;
                case Key.Back:
                    ListerManager.Instance.CommandManager.DirectoryUp();
                    break;
                default:
                    return false;
            }
            return true;
        }

    }
}
