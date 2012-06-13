using System.Windows;

namespace Kex.Controller.Commands.Window
{
    public class WindowRestoreCommand : BaseCommand
    {
        public WindowRestoreCommand(ListerManager listerManager) : base(listerManager)
        {
        }

        public override string Id
        {
            get { return "WindowRestore"; }
        }

        public override string Name
        {
            get { return "Restore Window"; }
        }

        public override string Description
        {
            get { return "Restores Window Position"; }
        }

        public override void Execute()
        {
            Application.Current.MainWindow.WindowState = WindowState.Maximized;            
        }
    }
}
