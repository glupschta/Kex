using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Kex.Common;
using Kex.Controller;
using Kex.Model.ItemProvider;

namespace Kex.Model.Lister
{
    public class FileLister : BaseLister
    {
        public FileLister() : base()
        {
            ItemProvider = new FilesystemItemProvider();
        }

        public FileLister(ILister lister) : this()
        {
            NavigationHistory = lister.NavigationHistory;
            
        }

        public FileLister(ILister lister, string directory)
        {
            NavigationHistory = lister.NavigationHistory;
            ItemProvider = new FilesystemItemProvider();
            CurrentDirectory = directory;
        }

        protected readonly IItemProvider<FileItem> ItemProvider;

        public override void Refresh()
        {
            Items = ItemProvider.GetItems(CurrentDirectory);
        }


        public override string XamlView
        {
            get { return "fullView"; }
        }

        public override void DoAction(object obj)
        {
            var item = obj as FileItem;
            var currentItem = item.Properties;
            if (currentItem == null) return;

            if (currentItem.ShellObject != null && currentItem.ShellObject.IsLink)
            {
                item.FullPath = ((string)currentItem.ShellObject.Properties.GetProperty("System.Link.TargetParsingPath").ValueAsObject);
                if (item.FullPath == null)
                {
                    item.FullPath = currentItem.ShellObject.Properties.System.ParsingPath.Value;
                }
                item.ItemType = Directory.Exists(item.FullPath) ? ItemType.Container : ItemType.Executable;
            }


            if (item.ItemType == ItemType.Container)
            {
                ListerManager.Instance.CommandManager.SetContainer(item.FullPath);
            }
            else
            {
                try
                {
                    if (item.FullPath.EndsWith(".kll"))
                    {
                        var locations = File.ReadAllLines(item.FullPath);
                        foreach (var location in locations)
                        {
                            ListerManager.Instance.ListerViewManager.OpenLister(location);
                        }
                        ListerManager.Instance.CommandManager.ClosePopup();
                    }
                    else
                    {
                        var start = new ProcessStartInfo(item.FullPath);
                        Process.Start(start);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
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
    }
}
