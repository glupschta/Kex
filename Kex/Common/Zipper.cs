using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using Kex.Model;

namespace Kex.Common
{
    public class Zipper
    {
        public Zipper(IEnumerable<IItem> items)
        {
            _items = items;
        }

        public IEnumerable<string> GetPaths()
        {
            IEnumerable<string> _resultPaths = null;
            if (SingleItem && HasContainers)
            {
                _resultPaths = zipSingleFolder();
            }
            else if (HasContainers)
            {
                _resultPaths = zipSelectionWithContainers();
            }
            else
            {
                _resultPaths = _items.Select(i => i.FullPath);
            }
            return _resultPaths;
        }

        private IEnumerable<string> zipSelectionWithContainers()
        {
            throw new NotImplementedException();
        }

        private IEnumerable<string> zipSingleFolder()
        {
            var di = new DirectoryInfo(_items.First().FullPath);
            var destination = Path.Combine(Path.GetTempPath(), di.Name + ".zip");
            var package = ZipPackage.Open(destination);
        }

        private bool HasContainers
        {
             get { return _items.Any(i => i.ItemType == ItemType.Container); }
        }

        private bool SingleItem
        {
            get { return _items.Count() == 1; }
        }


        private readonly IEnumerable<IItem> _items;
    }
}
