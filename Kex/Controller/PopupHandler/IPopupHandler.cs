using System;
using System.Collections.Generic;
using System.Windows.Input;
using Kex.Common;

namespace Kex.Controller.PopupHandler
{
    public interface IPopupHandler
    {
        string Name { get; }
        IEnumerable<string> ListItems { get; }
        Func<string, string, bool> Filter { get; }
        void ItemSelected(string item);
        void HandleKey(object sender, KeyEventArgs e);
        void TextChanged(string text);
    }
}
