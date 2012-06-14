using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using Kex.Common;
using Kex.Views;

namespace Kex.Controller.Popups
{
    public abstract class BasePopup : INotifyPropertyChanged
    {
        protected readonly IPopupInput Input;
        protected BasePopup(IPopupInput input)
        {
            Input = input;
            input.Closed += input_Closed;
            Input.ListBox.DataContext = this;
        }

        void input_Closed(object sender, System.EventArgs e)
        {
            UnregisterHandlers();
            ListerManager.Instance.ListerViewManager.CurrentListerView.View.Background = Brushes.White;
            ListerManager.Instance.CommandManager.FocusView();
        }

        protected abstract void HandleSelection();

        public abstract string Name { get; }

        public string Text
        {
            get { return Input.TextBox.Text; }
            set { Input.TextBox.Text = value; }
        }

        public void ApplyListDefaultFilter(IEnumerable<IPopupItem> items)
        {
            ListItems = new ItemFilter(items, Text);
        }

        public IEnumerable<IPopupItem> ListItems
        {
            get { return _listItems; }
            set
            {
                _listItems = value;
                OnNotifyPropertyChanged("ListItems");
            }
        }

        public virtual void Show()
        {
            Input.Header = Name;
            Input.TextBox.Clear();
            Input.Show();
            Input.TextBox.Focus();
            RegisterHandlers();
            ListerManager.Instance.ListerViewManager.CurrentListerView.View.Background = new SolidColorBrush(Color.FromRgb(250, 250, 250));
        }

        public virtual void Hide()
        {
            Input.Hide();
        }

        protected virtual void RegisterHandlers()
        {
            Input.TextBox.KeyDown += TextBox_KeyDown;
            Input.TextBox.PreviewKeyDown += TextBox_PreviewKeyDown;
            Input.TextBox.TextChanged += TextBox_TextChanged;
            Input.ListBox.SelectionChanged += ListBox_SelectionChanged;
        }

        protected virtual void UnregisterHandlers()
        {
            Input.TextBox.KeyDown -= TextBox_KeyDown;
            Input.TextBox.PreviewKeyDown -= TextBox_PreviewKeyDown;
            Input.TextBox.TextChanged -= TextBox_TextChanged;
            Input.ListBox.SelectionChanged -= ListBox_SelectionChanged;
        }

        void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Down:
                    MoveDownInList();
                    e.Handled = true;
                    break;
                case Key.Up:
                    MoveUpInList();
                    e.Handled = true;
                    break;
            }
        }

        protected virtual void ListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
        }

        protected virtual void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
        }

        protected void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Handled) return;
            var ctrl = (e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
            var shift = (e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;

            e.Handled = true;
            switch (e.Key)
            {
                case Key.Oem102: //<
                case Key.Escape:
                    Hide();
                    break;
                case Key.Return:
                    HandleSelection();
                    break;
                case Key.Tab:
                    if (shift)
                        MoveUpInList();
                    else
                        MoveDownInList();
                    break;
                case Key.J:
                    if (ctrl)
                    {
                       MoveDownInList(); 
                    }
                    else
                    {
                        e.Handled = false;
                    }
                    break;
                case Key.K:
                    if (ctrl)
                    {
                        MoveUpInList();
                    }
                    else
                    {
                        e.Handled = false;
                    }
                    break;
                case Key.OemComma:
                    Input.TextBox.Clear();
                    break;
                default:
                    e.Handled = false;
                    var ignoredKeys = new List<Key> { Key.LeftAlt, Key.LeftCtrl, Key.LeftCtrl, Key.RightAlt, Key.RightCtrl, Key.RightShift };
                    if (!ignoredKeys.Contains(e.Key) && ctrl)
                    {
                        CommandKeyHandler.HandleKey(e, true);
                    }
                    break;
            }
        }

        public T GetSelectedListItem<T>()
            where T: class
        {
            return Input.ListBox.SelectedItem as T;
        }

        private void MoveUpInList()
        {
            if (Input.ListBox == null) return;
            
            var ind = Input.ListBox.SelectedIndex;
            ind = (ind > 0) ? --ind : Input.ListBox.Items.Count - 1;
            Input.ListBox.SelectedIndex = ind;
            Input.ListBox.ScrollIntoView(Input.ListBox.SelectedItem);
        }

        private void MoveDownInList()
        {
            if (Input.ListBox == null) return;

            var ind = Input.ListBox.SelectedIndex;
            ind = (ind < Input.ListBox.Items.Count - 1) ? ++ind : 0;
            Input.ListBox.SelectedIndex = ind;
            Input.ListBox.ScrollIntoView(Input.ListBox.SelectedItem);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnNotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        private IEnumerable<IPopupItem> _listItems;

    }

}
