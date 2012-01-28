using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Kex.Common;
using Kex.Controller;
using Kex.Controller.PopupHandler;
using Kex.Model.ItemProvider;
using Kex.Modell;
using Kex.Views;
using CommandManager = Kex.Controller.CommandManager;

namespace Kex
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Main;
        public MainWindow()
        {
            Main = this;
            InitializeComponent();
            tabControl.Items.Clear();
            var comManager = new CommandManager(listPopup);
            var tabManager = new TabbedListerViewManager(tabControl, listPopup);
            ListerManager.Initialize(comManager, tabManager);
            ListerManager.Instance.ListerViewManager.OpenLister(DriveInfo.GetDrives()[0].Name);
            Activated += MainView_Activated;
            this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);
        }

        void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            //if ListView has no Items, Event gets here
            if (e.OriginalSource is MainWindow)
                CommandKeyHandler.HandleKey(e);
        }

        void MainView_Activated(object sender, EventArgs e)
        {
            ListerManager.Instance.CommandManager.FocusView();
        }

        public static void Debug(params object[] entries)
        {
            foreach(var text in entries)
                Main.DebugBox.Text = text+Environment.NewLine+Main.DebugBox.Text; 
        }

    }
}
