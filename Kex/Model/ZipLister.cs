using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kex.Model.ItemProvider;

namespace Kex.Model
{
    public class ZipLister : FileLister
    {
        public ZipLister(ILister lister)
        {
            NavigationHistory = lister.NavigationHistory;
            ItemProvider = new ZipItemProvider();
        }

        public ZipLister(ILister lister, string directory)
        {
            NavigationHistory = lister.NavigationHistory;
            ItemProvider = new ZipItemProvider(directory);
        }

        public override string CurrentDirectory
        {

            get { return _currentDirectory; }
            set
            {
                _currentDirectory = value;
                _itemProvider = new ZipItemProvider(value);
                Refresh();
            }
        }
    }
}
