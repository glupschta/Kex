using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Kex.Views
{
    public interface IPopupInput
    {
        TextBox TextBox { get; }
        ListBox ListBox { get; }
        string Header { get; set; }
        void Show();
        void Hide();
        bool IsOpen { get; }
        event EventHandler Closed;
    }
}
