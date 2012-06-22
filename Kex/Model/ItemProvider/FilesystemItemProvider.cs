using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using Kex.Common;
using Kex.Controller;
using Kex.Model;
using Microsoft.WindowsAPICodePack.Shell;

namespace Kex.Model.ItemProvider
{
    public class FilesystemItemProvider : IItemProvider<FileProperties>, IDisposable
    {
        public FilesystemItemProvider()
        {
            ThumbnailFormatOption = ShellThumbnailFormatOption.IconOnly;
            ThumbnailRetrievalOption = ShellThumbnailRetrievalOption.Default;
        }

        public FilesystemItemProvider(string directory) : this()
        {
            CurrentContainer = directory;
        }

        public string CurrentContainer
        {
            get; set; 
        }

        protected IEnumerable<IItem<FileProperties>> currentItems;

        protected virtual IEnumerable<IItem<FileProperties>> GetItemsEnumerable()
        {
            var items = new List<IItem<FileProperties>>();
            if (CurrentContainer == null) return items;

            if (Directory.Exists(CurrentContainer))
            {
                items.AddRange(Directory.EnumerateDirectories(CurrentContainer)
                    .Select(di => getFileItemSafe(di, ItemType.Container)));
                items.AddRange(Directory.EnumerateFiles(CurrentContainer).
                    Select(fi => getFileItemSafe(fi, ItemType.Executable)));
            }
            else
            {
                try
                {
                    string serverName;
                    if (CurrentContainer.StartsWith(@"\\"))
                        serverName = CurrentContainer.Substring(2);
                    else
                    {
                        serverName = CurrentContainer;
                        CurrentContainer = @"\\" + CurrentContainer;
                    }
                    var share = new NetWorkShare();
                    var shares = share.GetShares(serverName);
                    items.AddRange(
                        shares.Select(
                            lo => new FileItem(CurrentContainer + "\\" + lo.shi1_netname, ItemType.Container, this)));
                } catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error");
                }
            }
            var ret = items.Where(i => i != null);
            if (!ret.Any())
            {
                ret = new List<FileItem> { new FileItem(CurrentContainer + "\\..", ItemType.Container, this) };
            }
            return ret;
        }

        private FileItem getFileItemSafe(string path, ItemType type)
        {
            if (type == ItemType.Container)
            {
                var di = new DirectoryInfo(path);
                if (!ShowHiddenItems && (di.Attributes & FileAttributes.Hidden) != 0) return null;
                return new FileItem(path, type, this);
            }

            var fi = new FileInfo(path);
            return (!ShowHiddenItems && (fi.Attributes & FileAttributes.Hidden) != 0) ? null : new FileItem(path, type, this);
        }

        public IEnumerable<IItem<FileProperties>> GetItems()
        {
            var items = GetItemsEnumerable();
            FetchDetails(items.FirstOrDefault()); //for grid column widths
            return items;
        }

        public FileProperties FetchDetails(IItem<FileProperties> item)
        {
            if (item == null) return null;

            var props = new FileProperties();
            props.ShellObject = ShellObject.FromParsingName(item.FullPath);
            props.Length = (long) (props.ShellObject.Properties.System.Size.Value ?? 0);
            props.Created = props.ShellObject.Properties.System.DateCreated.Value;
            props.LastModified = props.ShellObject.Properties.System.DateCreated.Value;

            props.ShellObject.Thumbnail.FormatOption = ThumbnailFormatOption;
            props.ShellObject.Thumbnail.RetrievalOption = ThumbnailRetrievalOption;
            props.Thumbnail = props.ShellObject.Thumbnail.SmallBitmapSource;
            props.Thumbnail.Freeze();
            item.Properties = props;
            return props;
        }

     

        public ShellThumbnailFormatOption ThumbnailFormatOption { get; set; }
        public ShellThumbnailRetrievalOption ThumbnailRetrievalOption { get; set; }
        public bool ShowHiddenItems { get; set; }

        public virtual void DoAction(IItem item)
        {
            var currentItem = ((FileItem)item).Properties;
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
                } catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }

        public void Dispose()
        {
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
                            new Column("Type", "Properties.ShellObject.Properties.System.ItemTypeText.Value"),
                            new Column("Length", "Properties.Length"),
                        }
                );
            }
        }

        private IEnumerable<Column> _columns;
    }

}
