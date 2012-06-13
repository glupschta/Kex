using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Kex.Controller.Commands
{
    public class UpdateColumnWidthCommand : BaseCommand
    {
        public UpdateColumnWidthCommand(ListerManager listerManager) : base(listerManager)
        {
        }

        public override string Id
        {
            get { return "UpdateColumnWidth"; }
        }

        public override string Name
        {
            get { return "Update Column Width"; }
        }

        public override string Description
        {
            get { return "Sets the width of Columns to the minimum need size for visible items"; }
        }

        public override void Execute()
        {
            var gridView = CurrentView.View.View as GridView;
            if (gridView == null) return;
            foreach (var column in gridView.Columns.Where(co => Double.IsNaN(co.Width)))
            {
                column.Width = column.ActualWidth;
                column.Width = double.NaN;
            }
        }
    }
}
