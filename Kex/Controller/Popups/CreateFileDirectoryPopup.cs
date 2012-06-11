using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kex.Views;

namespace Kex.Controller.Popups
{
    public class CreateFileDirectoryPopup : BasePopup
    {
        public CreateFileDirectoryPopup(IPopupInput input) : base(input)
        {
        }

        protected override void HandleSelection()
        {
            
        }

        public override string Name
        {
            get { return "Create"; }
        }
    }
}
