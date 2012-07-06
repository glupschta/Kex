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
    /*
    public class AsyncFilesystemItemProvider : IItemProvider<FileProperties>, IDisposable
    {
        public AsyncFilesystemItemProvider()
        {
            _currentWorker = new BackgroundWorker { WorkerSupportsCancellation = true };
            ThumbnailFormatOption = ShellThumbnailFormatOption.IconOnly;
            ThumbnailRetrievalOption = ShellThumbnailRetrievalOption.Default;
        }

        public AsyncFilesystemItemProvider(string directory)
            : this()
        {
            CurrentContainer = directory;
        }

        public string CurrentContainer
        {
            get;
            set;
        }

        protected IEnumerable<IItem<FileProperties>> currentItems;

        protected virtual IEnumerable<IItem<FileProperties>> GetItemsEnumerable()
        {
            var items = new List<IItem<FileProperties>>();
            if (Directory.Exists(CurrentContainer))
            {
                items.AddRange(Directory.EnumerateDirectories(CurrentContainer)
                    .Select(di => getFileItemSafe(di, ItemType.Container)));
                items.AddRange(Directory.EnumerateFiles(CurrentContainer).
                    Select(fi => getFileItemSafe(fi, ItemType.Executable)));
            }
            else
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
                items.AddRange(shares.Select(lo => new FileItem(CurrentContainer + "\\" + lo.shi1_netname, ItemType.Container, this)));
            }

            return items.Where(i => i != null);
        }

        private FileItem getFileItemSafe(string path, ItemType type)
        {
            if (type == ItemType.Container)
            {
                try
                {
                    var di = new DirectoryInfo(path);
                    if ((di.Attributes & FileAttributes.Hidden) != 0) return null;
                    //di.EnumerateFiles().Any(); //Enumerate möglich?
                    return new FileItem(path, type, this);
                }
                catch
                {
                    return null;
                }
            }

            var fi = new FileInfo(path);
            return (fi.Attributes & FileAttributes.Hidden) != 0 ? null : new FileItem(path, type, this);
        }

        public IEnumerable<IItem<FileProperties>> GetItems()
        {
            var items = GetItemsEnumerable();
            FetchDetails(items.FirstOrDefault());
            try
            {
                if (items.Any())
                {
                    const int preload = 1;
                    foreach (var item in items.Take(preload))
                    {
                        FetchDetails(item);
                    }
                    FetchPropertiesAsync(items.Skip(preload));
                }
                else
                {
                    items = new List<FileItem> { new FileItem(CurrentContainer + "\\..", ItemType.Container, this) };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return items;
        }


        public FileProperties FetchDetails(IItem<FileProperties> item)
        {
            if (item == null) return null;

            var fi = item as FileItem;
            var props = new FileProperties();
            try
            {
                if (item.FullPath == null)
                {
                    return props;
                }
                props.ShellObject = ShellObject.FromParsingName(item.FullPath);
                props.Length = (long)(props.ShellObject.Properties.System.Size.Value ?? 0);
                props.Created = props.ShellObject.Properties.System.DateCreated.Value;
                props.LastModified = props.ShellObject.Properties.System.DateCreated.Value;

                props.ShellObject.Thumbnail.FormatOption = ThumbnailFormatOption;
                props.ShellObject.Thumbnail.RetrievalOption = ThumbnailRetrievalOption;
                props.Thumbnail = props.ShellObject.Thumbnail.SmallBitmapSource;
                props.Thumbnail.Freeze();
                item.Properties = props;
                fi.PropertiesChanged();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return props;
        }

        [STAThread]
        public void FetchPropertiesAsync(IEnumerable<IItem<FileProperties>> items)
        {
            if (_currentWorker.IsBusy)
            {
                _currentWorker.CancelAsync();
                _currentWorker = new BackgroundWorker { WorkerSupportsCancellation = true };
            }
            _currentWorker.DoWork += WorkerDoWork;
            _currentWorker.RunWorkerAsync(items);
        }

        private void WorkerDoWork(object sender, DoWorkEventArgs e)
        {
            var items = e.Argument as IEnumerable<IItem<FileProperties>>;
            if (items == null) return;
            foreach (var item in items)
            {
                if (((BackgroundWorker)sender).CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                FetchDetails(item);
            }
        }

        public ShellThumbnailFormatOption ThumbnailFormatOption { get; set; }

        public ShellThumbnailRetrievalOption ThumbnailRetrievalOption { get; set; }

        public virtual void DoAction(IItem item)
        {
            var currentItem = ((IItem<FileProperties>)item).Properties;
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
                    var start = new ProcessStartInfo(item.FullPath);
                    Process.Start(start);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }

        private BackgroundWorker _currentWorker;
        public void Dispose()
        {
            foreach (var item in currentItems)
            {
                if (item != null && item.Properties != null)
                    item.Properties.Dispose();
            }
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
      */
}
