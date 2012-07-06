using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Kex.Common;
using Kex.Model.ItemProvider;
using Kex.Model.Lister;
using Kex.Model.Zip;

namespace Kex.Model
{
    public class ZipLister : BaseLister
    {
        public ZipLister(ILister lister, string zipFile)
        {
            NavigationHistory = lister.NavigationHistory;
            ItemProvider = new ZipItemProvider(zipFile);
            Refresh();
        }

        public override void SelectionChanged(ListView view, SelectionChangedEventArgs selectionChangedEventArgs)
        {
        }

        private readonly ZipItemProvider ItemProvider;

        public override void Refresh()
        {
            Items = ItemProvider.GetItems(CurrentContainer);
        }

        public string CurrentContainer { get; set; }

        public override string XamlView
        {
            get { return null; }
        }

        public override void DoAction(object item)
        {
            var zipItem = item as ZipItem;
            if (zipItem == null) return;

        }

        public override IEnumerable<Column> Columns
        {
            get
            {
                return _columns ?? (
                    _columns = new[]                                              
                        {
                            new Column("Name", "Name"),
                            new Column("LastModified", "Properties.LastModified"),
                            new Column("Type", "Properties.ShellObject.Properties.System.ItemTypeText.Value"),
                            new Column("Length", "Properties.Length"),
                        }
                );
            }
        }
        private IEnumerable<Column> _columns;
        private string currentZipFile;


    }
}
