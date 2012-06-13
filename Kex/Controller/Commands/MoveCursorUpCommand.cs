using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Kex.Controller.Commands
{
    public class MoveCursorUpCommand : BaseCommand
    {
        public MoveCursorUpCommand(ListerManager listerManager) : base(listerManager)
        {
        }

        public override string Id
        {
            get { return "CursorUp"; }
        }

        public override string Name
        {
            get { return "Cursor up"; }
        }

        public override string Description
        {
            get { return "Moves Cursor up"; }
        }

        public override void Execute()
        {
            var uie = Keyboard.FocusedElement as UIElement;
            if (uie == null) return;
            uie.MoveFocus(new TraversalRequest(FocusNavigationDirection.Up)); 
        }
    }
}
