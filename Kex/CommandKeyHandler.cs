using System.Windows.Input;
using Kex.Interfaces;

namespace Kex
{
    public class CommandKeyHandler
    {

        public bool HandleKey(ILister lister, Key k, bool shift, bool control, bool alt)
        {
            switch(k)
            {
                case Key.Space:
                    MessageHost.ViewHandler.MarkSelected();
                    break;
                case Key.Delete:
                    MessageHost.ViewHandler.Delete();
                    break;
                case Key.Escape:
                    MessageHost.ViewHandler.ClosePopup();
                    break;
                case Key.U:
                    if (shift)
                        MessageHost.ViewHandler.HistoryBack();
                    else
                        MessageHost.ViewHandler.DirectoryUp();
                    break;
                case Key.A:
                    if (shift)
                        MessageHost.ViewHandler.ClearSelection();
                    else
                        MessageHost.ViewHandler.SelectAll();
                    break;
                case Key.C:
                    MessageHost.ViewHandler.Copy();
                    break;
                case Key.X:
                    MessageHost.ViewHandler.Cut();
                    break;
                case Key.V:
                    MessageHost.ViewHandler.Paste();
                    break;
                case Key.Return:
                    MessageHost.ViewHandler.DoDefaultAction();
                    break;
                case Key.O:
                    MessageHost.ViewHandler.ShowEnterUrlPopup();
                    break;
                case Key.I:
                    MessageHost.ViewHandler.ShowBrowsingPopup();
                    break;
                case Key.H:
                    MessageHost.ViewHandler.GoLeft();
                    break;
                case Key.L:
                    MessageHost.ViewHandler.GoRight();
                    break;
                case Key.J:
                    if (shift)
                    {
                        MessageHost.ViewHandler.MarkSelected();
                        MessageHost.ViewHandler.GoDown();
                    }
                    else
                        MessageHost.ViewHandler.GoDown();
                    break;
                case Key.K:
                    if (shift)
                    {
                        MessageHost.ViewHandler.MarkSelected();
                        MessageHost.ViewHandler.GoUp();
                    }
                    else
                        MessageHost.ViewHandler.GoUp();
                    break;
                case Key.D:
                    MessageHost.ViewHandler.ShowDrivesPopup();
                    break;
                case Key.Q:
                        MessageHost.ViewHandler.FitWidthToListers();
                    break;
                case Key.E:
                    WindowsFunctions.ShowFileProperties(MessageHost.ViewHandler.CurrentItem.FullPath);
                    break;
                case Key.F:
                        MessageHost.ViewHandler.ShowFavorites();
                    break;
                case Key.G:
                    MessageHost.ViewHandler.ShowViewPopup();
                    break;
                case Key.M:
                    MessageHost.ViewHandler.ShowSpecialFolderPopup();
                    break;
                    break;
                case Key.P:
                    MessageHost.ViewHandler.CloseCurrentLister();
                    break;
                case Key.R:
                    MessageHost.ViewHandler.ShowContextMenu();
                    break;
                case Key.S:
                    MessageHost.ViewHandler.ShowSortPopup();
                    break;
                case Key.Z:
                    if (shift)
                        MessageHost.ViewHandler.SetFilter(null);
                    else
                        MessageHost.ViewHandler.ShowFilterPopup();
                    break;
                case Key.Tab:
                    if (shift)
                        MessageHost.ViewHandler.CycleListers(-1);
                    else
                        MessageHost.ViewHandler.CycleListers(1);
                    break;
                case Key.W:
                    MessageHost.ViewHandler.ShowListers();
                    break;
                case Key.Oem3:
                    MessageHost.ViewHandler.OpenLister();
                    break;
                case Key.Back:
                    MessageHost.ViewHandler.DirectoryUp();
                    break;
                default:
                    return false;
            }
            return true;
        }

        public string Name
        {
            get { return "Command"; }
        }

        public void Clear(ILister list){}
    }
}
