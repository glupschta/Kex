using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Kex.Common;
using Kex.Controller;
using System.Windows.Controls.Primitives;

namespace Kex.Views
{
    /// <summary>
    /// Interaction logic for TextInput.xaml
    /// </summary>
    public partial class ListboxTextInput : IPopupInput
    {
        public ListboxTextInput()
        {
            InitializeComponent();

            listView.PreviewGotKeyboardFocus += (sender, eventArgs) => eventArgs.Handled = true;
            popup.Closed += popup_Closed;
            input.FontFamily = Options.FontFamily;
            input.FontSize = Options.FontSize;
            FontFamily = Options.FontFamily;
            FontSize = Options.FontSize;

            var typeFace = new Typeface(Options.FontFamily.ToString());
            var ft = new FormattedText(" ", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeFace, Options.FontSize, Brushes.Black);
            input.Height = ft.Height+6;
            Loaded += (o,e) => SetPosition();
        }

        void popup_Closed(object sender, EventArgs e)
        {
            if (Closed != null)
            {
                Closed(this, EventArgs.Empty);
            }
        }

        public string Header
        {
            get { return head.Content as string; }
            set { head.Content = value; }
        }

        public TextBox TextBox
        {
            get { return input; }
        }

        public ListBox ListBox
        {
            get { return listView; }
        }

        public void Show()
        {
            popup.IsOpen = true;

        }

        public void Hide()
        {
            popup.IsOpen = false;
        }

        public bool IsOpen
        {
            get { return popup.IsOpen; }
        }

        public event EventHandler Closed;
        public void SetPosition()
        {
            var currentListerView = ListerManager.Instance.ListerViewManager.CurrentListerView;
            popup.Placement = PlacementMode.Right;
            popup.MaxHeight = currentListerView.ActualHeight;
            popup.HorizontalOffset = currentListerView.ActualWidth;
            popup.PlacementTarget = currentListerView;
            popup.VerticalOffset = 0;
        }
    }
}