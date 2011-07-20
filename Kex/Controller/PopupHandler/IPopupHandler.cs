using System.Collections.Generic;
using System.Windows.Input;
using Kex.Common;

namespace Kex.Controller.PopupHandler
{
    public interface IPopupHandler
    {
        string Name { get; }
        MatchMode MatchMode { get; }
        IEnumerable<string> ListItems { get; }
        void ItemSelected(string item);
        void HandleKey(object sender, KeyEventArgs e);
        void TextChanged(string text);
    }
}
