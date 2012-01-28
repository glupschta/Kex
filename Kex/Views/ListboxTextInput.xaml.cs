using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Kex.Common;
using Kex.Controller;
using Kex.Controller.PopupHandler;
using System.Windows.Controls.Primitives;

namespace Kex.Views
{
    /// <summary>
    /// Interaction logic for TextInput.xaml
    /// </summary>
    public partial class ListboxTextInput : INotifyPropertyChanged
    {
        public ListboxTextInput()
        {
            InitializeComponent();
            DataContext = this;
            input.KeyDown += ListboxTextInput_KeyDown;
            input.TextChanged += input_TextChanged;
            listView.PreviewGotKeyboardFocus += (sender, eventArgs) => eventArgs.Handled = true;
            listView.SelectionChanged += listView_SelectionChanged;
        }

        void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListSelectionChanged != null)
            {
                ListSelectionChanged(listView.SelectedItem as string);
            }
        }

        public IPopupHandler Handler { get; set; }

        public IEnumerable<string> ListItems
        {
            get { return listItems; }
            set
            {
                listItems = value;
                grid.RowDefinitions[1].Height = listItems != null && listItems.Any() ? new GridLength() : new GridLength(0);
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

        public void Show(bool keepText=false)
        {
            popup.IsOpen = true;
            input.Focus();
            Keyboard.Focus(input);
            if (!keepText)
            {
                Text = "";
            }
            else {inputChanged();}

            ListItems = Handler.ListItems;
            Filter = Handler.Filter ?? DefaultFilter;
            filterMatchingItems();    
            Header = Handler.Name;
            listView.SelectedIndex = 0;

            var currentListerView = ListerManager.Instance.ListerViewManager.CurrentListerView;
            popup.PlacementTarget = currentListerView;
            popup.Placement = PlacementMode.Relative;
            popup.HorizontalOffset = (currentListerView.ActualWidth - popup.Child.RenderSize.Width) / 2;
            popup.VerticalOffset = (currentListerView.ActualHeight - popup.Child.RenderSize.Height) / 2;
        }

        public void Close()
        {
            popup.IsOpen = false;
            SetFocusToView();
        }

        void input_TextChanged(object sender, TextChangedEventArgs e)
        {
            inputChanged();
            e.Handled = true;
        }

        void inputChanged()
        {
            Handler.TextChanged(Text);
            setListSelection();
            filterMatchingItems();
        }

        private void filterMatchingItems()
        {
            FilteredItems = ListItems == null ? null : ListItems.Where(li => Filter(li, Text));
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
                        var ignoredKeys = new List<Key> { Key.LeftAlt, Key.LeftCtrl, Key.LeftCtrl, Key.RightAlt, Key.RightCtrl, Key.RightShift };
                        if (!ignoredKeys.Contains(e.Key))
                        {
                            if (ctrl)
                            {
                                CommandKeyHandler.HandleKey(e, true);
                            }
                            e.Handled = false;
                        }
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
            listView.ScrollIntoView(listView.SelectedItem);
        }

        private void moveDownInList()
        {
            var ind = listView.SelectedIndex;
            if (ind < listView.Items.Count - 1)
            {
                ind++;
            }
            listView.SelectedIndex= ind;
            listView.ScrollIntoView(listView.SelectedItem);
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
            ListerManager.Instance.CommandManager.FocusView();
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
                listView.SelectedItem = ListItems.FirstOrDefault(li => Filter(li, Text));
            }
            else
            {
                listView.SelectedIndex = -1;
            }
        }


        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event Action<string> ListSelectionChanged;

        public Func<string, string, bool> Filter;

        private bool DefaultFilter(string source, string text)
        {
            if (string.IsNullOrEmpty(source)) return true;
            if (string.IsNullOrEmpty(text)) return true;
            return source.IndexOf(text, StringComparison.OrdinalIgnoreCase) > -1;
        }

        private IEnumerable<string> listItems;
        private IEnumerable<string> filteredItems;
        internal bool ignoreLostFocus;

    }
}
