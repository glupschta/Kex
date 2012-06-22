using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kex.Common;
using Kex.Model;
using Kex.Views;

namespace Kex.Controller.Popups
{
    public class FilterPopup : BasePopup
    {
        public FilterPopup(IPopupInput input) : base(input)
        {
            ListItems = FileTypes;
        }

        public override void Show()
        {
            base.Show();
            Text = ListerManager.Instance.ListerViewManager.CurrentListerView.Lister.Filter;
            Input.TextBox.CaretIndex = Text.Length;
        }

        public override void Hide()
        {
            base.Hide();
            ListerManager.Instance.ListerViewManager.CurrentListerView.View.SelectedIndex = 0;
            ListerManager.Instance.CommandManager.FocusView();
        }

        protected override void ListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (Input.ListBox.SelectedIndex != -1)
            {
                var selected = Input.ListBox.SelectedValue as IPopupItem;
                if (selected != null)
                    ListerManager.Instance.CommandManager.SetFilter(selected.FilterString); 
            }
        }

        protected override void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Input.ListBox.SelectedIndex = -1;
            ListerManager.Instance.CommandManager.SetFilter(Text);
            ApplyListDefaultFilter(FileTypes);
        }

        protected override void HandleSelection()
        {
            Hide();
        }

        public override string Name
        {
            get { return "Filter"; }
        }

        public IEnumerable<StringPopupItem> FileTypes
        {
            get
            {
                var types = ListerManager.Instance.ListerViewManager.CurrentListerView.Lister.Items.Cast<FileItem>()
                    .Select(fi => fi.Properties.ShellObject.Properties.System.FileExtension.Value)
                    .Where(ext => !string.IsNullOrEmpty(ext))
                    .Distinct().Select(t => new StringPopupItem(t));
                return types;
            }
        }
    }
}
