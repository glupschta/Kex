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

        public static void Initialize(CommandManager commandManager, IListerViewManager listerViewManager)
        {
            Instance = new ListerManager(commandManager, listerViewManager);
        }

        private ListerManager(CommandManager commandManager, IListerViewManager listerViewManager)
        {
            CommandManager = commandManager;
            ListerViewManager = listerViewManager;
        }

        public CommandManager CommandManager { get; private set; }
        public IListerViewManager ListerViewManager { get; private set; }
    }
}
