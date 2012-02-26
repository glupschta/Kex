using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using Kex.Common;
using Kex.Controller;
using Kex.Modell;
using Microsoft.WindowsAPICodePack.Shell;

namespace Kex.Model.ItemProvider
{
    public class FilesystemItemProvider : IItemProvider<FileProperties>, IDisposable
    {
        public FilesystemItemProvider()
        {
            _currentWorker = new BackgroundWorker {WorkerSupportsCancellation = true};
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
            if (Directory.Exists(CurrentContainer))
            {
                items.AddRange(Directory.EnumerateDirectories(CurrentContainer).Select(di => new FileItem(di, ItemType.Container, this)));
                items.AddRange(Directory.EnumerateFiles(CurrentContainer).Select(fi => new FileItem(fi, ItemType.Executable, this)));
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
            
            return items;
        }

        public IEnumerable<IItem<FileProperties>> GetItems()
        {
            if (currentItems != null)
            {
                Dispose();
            }
            var items = GetItemsEnumerable();
            try
            {
                if (items.Any())
                {
                    const int preload = 20;
                    foreach(var item in items.Take(preload))
                    {
                        FetchDetails(item);
                    }
                    FetchPropertiesAsync(items.Skip(preload));
                }
                else
                {
                    items = new List<FileItem> {new FileItem(CurrentContainer + "\\..", ItemType.Container, this)};
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            currentItems = items;
            return items;
        }

        public FileProperties FetchDetails(IItem<FileProperties> item)
        {
            var fi = item as FileItem;
            var props = new FileProperties();
            try
            {
                props.ShellObject = ShellObject.FromParsingName(item.FullPath);
                props.ShellObject.Thumbnail.FormatOption = ThumbnailFormatOption;
                props.ShellObject.Thumbnail.RetrievalOption = ThumbnailRetrievalOption;
                props.Length = (long) (props.ShellObject.Properties.System.Size.Value ?? 0);
                props.Created = props.ShellObject.Properties.System.DateCreated.Value;
                props.LastModified = props.ShellObject.Properties.System.DateCreated.Value;
                props.Thumbnail = props.ShellObject.Thumbnail.SmallBitmapSource;
                props.Thumbnail.Freeze();
                item.Properties = props;
                if (props.ShellObject != null && props.ShellObject.IsLink)
                {
                    item.FullPath = ((string)props.ShellObject.Properties.GetProperty("System.Link.TargetParsingPath").ValueAsObject);
                    item.ItemType = Directory.Exists(item.FullPath) ? ItemType.Container : ItemType.Executable;
                }
                fi.PropertiesChanged();
            } catch (Exception ex)
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
            //var currentItem = ((IItem<FileProperties>)item).Properties;
            //if (currentItem == null) return;
            //if (currentItem.ShellObject != null && currentItem.ShellObject.IsLink)
            //{
            //    var properties = currentItem.ShellObject.Properties;
            //    var target = ((string)properties.GetProperty("System.Link.TargetParsingPath").ValueAsObject);
            //    if (Directory.Exists(target))
            //    {
            //        ListerManager.Instance.CommandManager.SetContainer(target);
            //        return;
            //    }
            //}
            if (item.ItemType == ItemType.Container)
            {
                ListerManager.Instance.CommandManager.SetContainer(item.FullPath);
            }
            else
            {
                var start = new ProcessStartInfo(item.FullPath);
                Process.Start(start);
            }
        }

        private BackgroundWorker _currentWorker;
        public void Dispose()
        {
            foreach(var item in currentItems)
            {
                if (item != null && item.Properties != null)
                    item.Properties.Dispose();
            }
        }
    }

}
