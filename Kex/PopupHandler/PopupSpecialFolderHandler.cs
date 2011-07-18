using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Kex.Interfaces;
using Kex.Controller;

namespace Kex.PopupHandler
{
    public class PopupSpecialFolderHandler : IPopupHandler
    {
        private readonly Dictionary<string, string> specialFolders;

        public PopupSpecialFolderHandler()
        {
            var s = Enum.GetValues(typeof(Environment.SpecialFolder))
                .Cast<Environment.SpecialFolder>().Distinct();

            specialFolders = s.ToDictionary(sf => Enum.GetName(typeof(Environment.SpecialFolder), sf), Environment.GetFolderPath);
        }

        public string Name
        {
            get { return "Special Folders"; }
        }

        public MatchMode MatchMode
        {
            get { return MatchMode.Contains; }
        }

        public IEnumerable<string> ListItems
        {
            get { return specialFolders.Keys; }
        }

        public void ItemSelected(string item)
        {
            ListerManager.Manager.SetDirectory(specialFolders[item]);
        }

        public void HandleKey(object sender, KeyEventArgs e)
        {
        }

        public void TextChanged(string text)
        {
        }
    }
}
