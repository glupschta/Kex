using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Kex.Interfaces
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
