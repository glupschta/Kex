using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kex.Common;

namespace Kex.Controller
{
    public class ListerManager
    {
        public static ListerManager Instance { get; private set; }

        public static void Initialize(ICommandManager commandManager, IListerViewManager listerViewManager)
        {
            Instance = new ListerManager(commandManager, listerViewManager);
        }

        private ListerManager(ICommandManager commandManager, IListerViewManager listerViewManager)
        {
            CommandManager = commandManager;
            ListerViewManager = listerViewManager;
        }

        public ICommandManager CommandManager { get; private set; }
        public IListerViewManager ListerViewManager { get; private set; }
    }
}
