using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Kex.Controller.Commands
{
    public class MoveCursorDownCommand : BaseCommand
    {
        public MoveCursorDownCommand(ListerManager listerManager)
            : base(listerManager)
        {
        }

        public override string Id
        {
            get { return "CursorDown"; }
        }

        public override string Name
        {
            get { return "Cursor down"; }
        }

        public override string Description
        {
            get { return "Moves Cursor down"; }
        }

        public override void Execute()
        {
            var uie = Keyboard.FocusedElement as UIElement;
            if (uie == null) return;
            uie.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down)); 
        }
    }
}
