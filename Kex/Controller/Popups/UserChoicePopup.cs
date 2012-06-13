using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Kex.Common;
using Kex.Views;

namespace Kex.Controller.Popups
{
    public class UserChoicePopup : BasePopup
    {
        public UserChoicePopup(IPopupInput input, string question, IEnumerable<string> possibleAnswers) : base(input)
        {
            _name = question;
            _possibleAnswers = possibleAnswers.Select(pa => new StringPopupItem(pa));
            ListItems = _possibleAnswers;
            Input.ListBox.SelectedIndex = 0;
        }

        protected override void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ListItems = new ItemFilter(_possibleAnswers, Text);
            Input.ListBox.SelectedIndex = 0;
        }

        protected override void HandleSelection()
        {
            if (SelectionDone != null)
            {                                                                       
                var item = Input.ListBox.SelectedItem as StringPopupItem;
                SelectionDone(this, item.FilterString);
            }
            Hide();
        }

        public event SelectionDoneHandler SelectionDone;
        public delegate void SelectionDoneHandler(object sender, string selectedAnswer);

        private string _name;
        public override string Name { get { return _name; }}

        private IEnumerable<IPopupItem> _possibleAnswers;
    }
}
