using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Kex.Common;
using Kex.Interfaces;

namespace Kex.Views
{
    /// <summary>
    /// Interaction logic for TextInput.xaml
    /// </summary>
    public partial class ListboxTextInput : UserControl, INotifyPropertyChanged
    {
        public ListboxTextInput()
        {
            InitializeComponent();
            DataContext = this;
            input.KeyDown += ListboxTextInput_KeyDown;
            input.TextChanged += input_TextChanged;
            popup.LostKeyboardFocus += popup_LostFocus;
            listView.PreviewGotKeyboardFocus += (sender, eventArgs) => eventArgs.Handled = true;
        }

        public IPopupHandler Handler { get; set; }

        public IEnumerable<string> ListItems
        {
            get { return listItems; }
            set
            {
                listItems = value;
                OnPropertyChanged("ListItems");
            }
        }

        public IEnumerable<string> FilteredItems
        {
            get { return filteredItems; }
            set
            {
                filteredItems = value;
                OnPropertyChanged("FilteredItems");
            }
        }

        public void Show()
        {
            popup.IsOpen = true;
            input.Focus();
            Text = "";
            ListItems = Handler.ListItems;
            filterMatchingItems();
            grid.RowDefinitions[1].Height = ListItems != null && ListItems.Any() ? new GridLength() : new GridLength(0);
            Header = Handler.Name;
            listView.SelectedIndex = 0;
        }

        public void Close()
        {
            popup.IsOpen = false;
            SetFocusToView();
        }

        void popup_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ignoreLostFocus) return;
            Close();
        }

        void input_TextChanged(object sender, TextChangedEventArgs e)
        {
            setListSelection();
            filterMatchingItems();
            Handler.TextChanged(Text);
            e.Handled = true;
        }

        private void filterMatchingItems()
        {
            FilteredItems = ListItems == null ? ListItems : ListItems.Where(MatchesFilter);
        }

        void ListboxTextInput_KeyDown(object sender, KeyEventArgs e)
        {
            Handler.HandleKey(sender, e);
            if (e.Handled) return;
            var ctrl = (e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
            switch (e.Key)
            {
                case Key.Oem102:
                case Key.Escape:
                    closePopup();
                    e.Handled = true;
                    break;
                case Key.Return:
                    closeAndHandleSelection();
                    e.Handled = true;
                    break;
                default:
                    if (e.Key == Key.J && ctrl)
                    {
                        moveDownInList();
                        e.Handled = true;
                    }
                    else if (e.Key == Key.K && ctrl)
                    {
                        moveUpInList();
                        e.Handled = true;
                    }
                    else
                    {
                        e.Handled = false;
                    }
                    break;
            }
        }

        private void moveUpInList()
        {
            var ind = listView.SelectedIndex;
            if (ind > 0)
            {
                ind--;
            }
            listView.SelectedIndex = ind;
        }

        private void moveDownInList()
        {
            var ind = listView.SelectedIndex;
            if (ind < listView.Items.Count - 1)
            {
                ind++;
            }
            listView.SelectedIndex= ind;
        }

        private void closeAndHandleSelection()
        {
            Close();
            var selection = listView.SelectedItem as string ?? Text;
            Handler.ItemSelected(selection);
            SetFocusToView();
        }

        private void closePopup()
        {
            Close();
            SetFocusToView();
        }

        private static void SetFocusToView()
        {
            MessageHost.ViewHandler.FocusView();
        }

        public string Header
        {
            get { return head.Content as string; }
            set { head.Content = value; }
        }

        public string Text
        {
            get { return input.Text; }
            set
            {
                input.Text = value;
                setListSelection();
                OnPropertyChanged("Text");
            }
        }

        private void setListSelection()
        {
            if (ListItems == null || !ListItems.Any())
                return;
            if (!string.IsNullOrEmpty(Text))
            {
                listView.SelectedItem = ListItems.FirstOrDefault(MatchesFilter);
            }
            else
            {
                listView.SelectedIndex = -1;
            }
        }


        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private bool MatchesFilter(string source)
        {
            if (string.IsNullOrEmpty(Text)) return true;
            return Handler.MatchMode == MatchMode.StartsWith
                       ? source.StartsWith(Text, StringComparison.OrdinalIgnoreCase)
                       : source.IndexOf(Text, StringComparison.OrdinalIgnoreCase) > -1;
        }

        private IEnumerable<string> listItems;
        private IEnumerable<string> filteredItems;
        internal bool ignoreLostFocus;

    }
}
