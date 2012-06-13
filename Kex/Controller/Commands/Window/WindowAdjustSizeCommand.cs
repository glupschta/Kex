using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Kex.Controller.Commands.Window
{
    public class WindowAdjustSizeCommand : BaseCommand
    {
        public WindowAdjustSizeCommand(ListerManager listerManager) : base(listerManager)
        {
        }

        public override string Id
        {
            get { return "WindowAdjustSize"; }
        }

        public override string Name
        {
            get { return "Adjust Window Size"; }
        }

        public override string Description
        {
            get { return "Sets the Size of the Window to the minimal no Scrollvar value"; }
        }

        public override void Execute()
        {
            new UpdateColumnWidthCommand(Manager).Execute();
            var grid = CurrentView.View.View as GridView;
            if (grid != null)
            {
                var colWidth = grid.Columns.Sum(c => c.ActualWidth);
                Application.Current.MainWindow.Width = colWidth + 80;
            }
        }


    }
}
