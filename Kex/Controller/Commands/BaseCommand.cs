using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kex.Views;

namespace Kex.Controller.Commands
{
    public abstract class BaseCommand
    {
        private readonly ListerManager _listerManager;
        protected BaseCommand(ListerManager listerManager)
        {
            _listerManager = listerManager;
        }

        public ListerManager Manager { get { return _listerManager; } }

        public ListerView CurrentView { get { return Manager.ListerViewManager.CurrentListerView; } }

        public abstract string Id { get;  }
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract void Execute();
    }
}
