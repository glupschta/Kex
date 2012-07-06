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
            this.Loaded += new RoutedEventHandler(PropertyDialog_Loaded);
        }

        void PropertyDialog_Loaded(object sender, RoutedEventArgs e)
        {
            PropertyList.SelectedIndex = 0;
            this.PropertyList.Focus();  
            Keyboard.Focus(PropertyList.ItemContainerGenerator.ContainerFromItem(PropertyList.SelectedItem) as IInputElement);
        }

        void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
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
            var items = new List<PropertyItem>();
            items.AddRange(fi.ShellObject.Properties.DefaultPropertyCollection
                .Where(pc => pc.CanonicalName != null && pc.ValueAsObject != null)
                .Select(pc => new PropertyItem(pc.Description.DisplayName, pc.ValueAsObject.ToString())));

            if (fi.FullPath.ToLower().EndsWith(".dll"))
            {
                try
                {
                    items.Add(new PropertyItem("References", ""));
                    var a = Mono.Cecil.AssemblyDefinition.ReadAssembly(fi.FullPath);
                    items.AddRange(a.MainModule.AssemblyReferences.Select(ar => new PropertyItem("", ar.FullName)));
                } catch {}
            }
            PropertyList.ItemsSource = items;
            Title = fi.Name + " " + "Properties";
                
            ShowDialog();
        }
    }

    public class PropertyItem
    {
        public PropertyItem (string key, string value)
        {
            Key = key;
            Value = value;
        }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
