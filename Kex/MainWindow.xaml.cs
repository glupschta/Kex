using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Kex.Common;
using Kex.Controller;
using Kex.Model.ItemProvider;
using Kex.Model;
using Kex.Views;
using CommandManager = Kex.Controller.CommandManager;

namespace Kex
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var tabManager = new TabbedListerViewManager(tabControl, listPopup);
            var comManager = new CommandManager(listPopup, tabManager);
            
            ListerManager.Initialize(comManager, tabManager);
            Activated += MainView_Activated;
            OpenLister();
        }

        void MainView_Activated(object sender, EventArgs e)
        {
            ListerManager.Instance.CommandManager.FocusView();
        }

        public void OpenLister()
        {
            var arguments = ((string[])App.Current.Properties["StartupArguments"]);
            var filename = arguments.Length > 0 ? arguments[0] : null;
            if (filename == null)
                ListerManager.Instance.ListerViewManager.OpenLister(DriveInfo.GetDrives()[0].Name);
            else
            {
                var locations = File.ReadAllLines(filename);
                foreach (var location in locations)
                {
                    ListerManager.Instance.ListerViewManager.OpenLister(location);
                }
            }
        }

        public static void Debug(params object[] entries)
        {
            var main = ((MainWindow) App.Current.MainWindow);
            if (main.DebugBox.ActualHeight == 0) return;

            foreach(var text in entries)
                main.DebugBox.Text = text+Environment.NewLine+main.DebugBox.Text; 
        }

    }
}
