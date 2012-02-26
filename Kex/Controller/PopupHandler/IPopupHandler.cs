using System;
using System.Collections.Generic;
using System.Windows.Input;
using Kex.Common;

namespace Kex.Controller.PopupHandler
{
    public interface IPopupHandler<T>
    {
        string Name { get; }
        IEnumerable<T> ListItems { get; }
        Func<T, string, bool> Filter { get; }
        void ItemSelected(T item);
        void HandleKey(object sender, KeyEventArgs e);
        void TextChanged(string text);
        bool SetSelectionInListView { get; }
    }
}
