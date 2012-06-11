using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Kex.Model;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

namespace Kex.Views
{
    /// <summary>
    /// Interaction logic for PropertyDialog.xaml
    /// </summary>
    public partial class PropertyDialog : Window
    {
        public PropertyDialog()
        {
            InitializeComponent();
            KeyDown += PropertyDialog_KeyDown;
            OkButton.Click += OkButton_Click;
        }

        void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        void PropertyDialog_KeyDown(object sender, KeyEventArgs e)
        {
            var uie = Keyboard.FocusedElement as UIElement;
            switch (e.Key)
            {
                case Key.J:
                    uie.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                    e.Handled = true;
                    break;
                case Key.K:
                    uie.MoveFocus(new TraversalRequest(FocusNavigationDirection.Up));
                    e.Handled = true;
                    break;
                case Key.Escape:
                    Close();
                    break;
            }
        }

        public void Show(FileItem fi)
        {
            var items = fi.ShellObject.Properties.DefaultPropertyCollection;
            PropertyList.ItemsSource = items;
            Title = fi.Name + " " + "Properties";
            PropertyList.SelectedIndex = 0;
            this.PropertyList.Focus();            
            Keyboard.Focus(PropertyList.ItemContainerGenerator.ContainerFromItem(PropertyList.SelectedItem) as IInputElement);
            ShowDialog();
        }
    }
}
