using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Kex.Controller;
using Kex.Model.Item;

namespace Kex.Model.Lister
{
    public class TextLister : BaseLister
    {
        private readonly FileItem FileItem;

        public TextLister(FileItem fileItem)
            : base()
        {
            FileItem = fileItem;
        }

        public override void SelectionChanged(System.Windows.Controls.ListView view, System.Windows.Controls.SelectionChangedEventArgs selectionChangedEventArgs)
        {

        }

        public override void Refresh()
        {
            if (FileItem == null) return;
            var items = new List<TextItem>();

            try
            {
                int i = 1;
                items = File.ReadAllLines(FileItem.FullPath).Select(l => new TextItem(i++, l)).ToList();
            } catch (Exception ex)
            {
                //TODO: Show Exception
            }

            Items = items;
        }

        public override string XamlView
        {
            get { return null; }
        }

        public override void DoAction(object obj)
        {
        }

        private IEnumerable<Column> columns;
        public override IEnumerable<Column> Columns
        {
            get
            {
                return columns ?? (columns = new[]
                                                   { 
                                                       new Column("Line", "LineNumber"), 
                                                       new Column("Content", "DisplayName")
                                                   });
            }
        }
    }

}
