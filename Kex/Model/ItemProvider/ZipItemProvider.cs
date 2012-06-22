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

        public IEnumerable<Column> Columns
        {
            get
            {
                return _columns ?? (
                    _columns = new[]
                        {
                            new Column("Name", "Name"),
                            new Column("LastModified", "Properties.LastModified"),
                            new Column("Length", "Properties.Length"),
                        }
                );
            }
        }

        private IEnumerable<Column> _columns;

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
