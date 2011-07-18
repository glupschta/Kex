using System;
using System.Windows;
using Kex.Common;
using Kex.Controller;

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
            ListerManager.Initialize(header, stackPanel, listPopup);
            MessageHost.ViewHandler = ListerManager.Manager;
            ListerManager.Manager.OpenLister(@"C:\");
            Activated += MainView_Activated;
        }

        void MainView_Activated(object sender, EventArgs e)
        {
            MessageHost.ViewHandler.FocusView();
        }
    }
}
