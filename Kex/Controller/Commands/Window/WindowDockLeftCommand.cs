using System.Windows;

namespace Kex.Controller.Commands.Window
{
    public class WindowDockLeftCommand : BaseCommand
    {
        public WindowDockLeftCommand(ListerManager listerManager) : base(listerManager)
        {
        }

        public override string Id
        {
            get { return "WindowDockLeft"; }
        }

        public override string Name
        {
            get { return "Dock window left"; }
        }

        public override string Description
        {
            get { return "Docks Window on the left Side"; }
        }

        public override void Execute()
        {
            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;
            Application.Current.MainWindow.Top = 0;
            Application.Current.MainWindow.Left = 0;
            Application.Current.MainWindow.Width = screenWidth / 2;
            Application.Current.MainWindow.Height = screenHeight;
        }
    }
}
