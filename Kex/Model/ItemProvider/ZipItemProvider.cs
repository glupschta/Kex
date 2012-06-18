using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Ionic.Zip;

namespace Kex.Model.ItemProvider
{
    public class ZipItemProvider : IItemProvider<FileProperties>
    {
        public ZipItemProvider() {}
        public ZipItemProvider(string directory): base()
        {
            CurrentContainer = directory;
        }

        public Dictionary<string, string> Columns
        {
            get
            {
                if (columns == null)
                {
                    columns = new Dictionary<string, string>();
                    columns.Add("Name", "Name");
                    columns.Add("LastModified", "Properties.LastModified");
                    columns.Add("Length", "Properties.Length");
                }
                return columns;
            }
        }

        private Dictionary<string, string> columns;

        public string CurrentContainer { get; set; }
        public void DoAction(IItem item)
        {
        }

        public IEnumerable<IItem<FileProperties>> GetItems()
        {
            var zip = new ZipFile(CurrentContainer);
            var ret = zip.Entries.Select(zi => new ZipItem(zi)).ToList();
            var grouped = zip.Entries.GroupBy(ze => Path.GetDirectoryName(ze.FileName)).ToList();
            Console.WriteLine(grouped);
            return ret;
        }

        public FileProperties FetchDetails(IItem<FileProperties> item)
        {
            return new FileProperties();
        }

    }
}
