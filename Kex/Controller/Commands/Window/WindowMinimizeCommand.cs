using System.Windows;

namespace Kex.Controller.Commands.Window
{
    public class WindowMinimizeCommand : BaseCommand
    {
        public WindowMinimizeCommand(ListerManager listerManager) : base(listerManager)
        {
        }

        public override string Id
        {
            get { return "WindowMinimize"; }
        }

        public override string Name
        {
            get { return "Minimize Window"; }
        }

        public override string Description
        {
            get { return "Minimizes Main Window"; }
        }

        public override void Execute()
        {
            Application.Current.MainWindow.WindowState = WindowState.Maximized;
        }
    }
}
