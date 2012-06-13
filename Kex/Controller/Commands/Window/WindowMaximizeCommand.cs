using System.Windows;

namespace Kex.Controller.Commands.Window
{
    public class WindowMaximizeCommand : BaseCommand
    {
        public WindowMaximizeCommand(ListerManager listerManager) : base(listerManager)
        {
        }

        public override string Id
        {
            get { return "WindowMaximize"; }
        }

        public override string Name
        {
            get { return "Maximize Window"; }
        }

        public override string Description
        {
            get { return "Maximizes Main Window"; }
        }

        public override void Execute()
        {
            Application.Current.MainWindow.WindowState = WindowState.Maximized;
        }
    }
}
