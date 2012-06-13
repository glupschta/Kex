using System.Windows;

namespace Kex.Controller.Commands.Window
{
    public class WindowDockRightCommand : BaseCommand
    {
        public WindowDockRightCommand(ListerManager listerManager) : base(listerManager)
        {
        }

        public override string Id
        {
            get { return "Dock Window Right"; }
        }

        public override string Name
        {
            get { return "Docks Window Right"; }
        }

        public override string Description
        {
            get { return "Docks window on the right side"; }
        }

        public override void Execute()
        {
            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;
            Application.Current.MainWindow.Top = 0;
            Application.Current.MainWindow.Left = screenWidth - screenWidth / 2;
            Application.Current.MainWindow.Width = screenWidth - Application.Current.MainWindow.Left;
            Application.Current.MainWindow.Height = screenHeight; 
        }
    }
}
