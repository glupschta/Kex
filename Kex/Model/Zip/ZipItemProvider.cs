using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ionic.Zip;
using Kex.Model.ItemProvider;

namespace Kex.Model.Zip
{
    public class ZipItemProvider : IItemProvider<ZipItem>
    {
        public ZipItemProvider(string zipFile)
        {
            ZipFile = zipFile;
            Zip = new ZipFile(zipFile);
        }

        public void DoAction(ZipItem item)
        {
        }

        public ZipItem FetchDetails(ZipItem item)
        {
            return item;
        }

        public IEnumerable<ZipItem> GetItems(string container)
        {
            container = container ?? "";

            var ret = Zip.Entries
                .Where(e => e.FileName.StartsWith(container))
                .Select(zi => new ZipItem(zi)).ToList();
            return ret;
        }

        private readonly string ZipFile;
        private readonly ZipFile Zip;
    }
}
