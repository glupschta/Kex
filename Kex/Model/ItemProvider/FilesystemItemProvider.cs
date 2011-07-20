using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Kex.Common;
using Kex.Modell;
using Microsoft.WindowsAPICodePack.Shell;

namespace Kex.Model.ItemProvider
{
    public class FilesystemItemProvider : IItemProvider<FileProperties>
    {
        public FilesystemItemProvider()
        {
            _currentWorker = new BackgroundWorker();
            _currentWorker.WorkerSupportsCancellation = true;
        }

        public FilesystemItemProvider(string directory) : this()
        {
            CurrentContainer = directory;
        }

        public string CurrentContainer
        {
            get; set; 
        }

        protected virtual IEnumerable<IItem> GetItemsEnumerable()
        {
            var allItems = new List<IItem>();
            allItems.AddRange(Directory.EnumerateDirectories(CurrentContainer).Select(di => new FileItem(di, ItemType.Container, this)));
            allItems.AddRange(Directory.EnumerateFiles(CurrentContainer).Select(fi => new FileItem(fi, ItemType.Executable, this)));
            return allItems;
        }

        public IEnumerable<IItem> GetItems()
        {
            var allItems = GetItemsEnumerable();
            try
            {
                if (allItems.Any())
                {
                    Fetch(allItems.First());
                    FetchPropertiesAsync(allItems);
                }
            }
            catch (Exception ex)
            {
                MessageHost.ViewHandler.HandleException(ex);
            }
            return allItems;
        }

        public FileProperties Fetch(IItem item)
        {
            var props = new FileProperties();
            try
            {
                props.ShellObject = ShellObject.FromParsingName(item.FullPath);
                props.ShellObject.Thumbnail.FormatOption = ThumbnailFormatOption;
                props.ShellObject.Thumbnail.RetrievalOption = ThumbnailRetrievalOption;
                props.Length = (long) (props.ShellObject.Properties.System.Size.Value ?? 0);
                props.Created = props.ShellObject.Properties.System.DateCreated.Value;
                props.LastModified = props.ShellObject.Properties.System.DateCreated.Value;
                props.Thumbnail = props.ShellObject.Thumbnail.MediumBitmapSource;
                props.Thumbnail.Freeze();
                ((IVirtualizedPropertyProvider<FileProperties>) item).Loaded = true;
            } catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            return props;
        }

        [STAThread]
        public void FetchPropertiesAsync(IEnumerable<IItem> items)
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
            _shellError = false;
            var items = e.Argument as IEnumerable<IItem>;
            if (items == null) return;
            foreach (var item in items)
            {
                if (((BackgroundWorker)sender).CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                Fetch(item);
            }
        }

        protected virtual ShellThumbnailFormatOption ThumbnailFormatOption
        {
            get { return ShellThumbnailFormatOption.Default; }
        }

        protected virtual ShellThumbnailRetrievalOption ThumbnailRetrievalOption
        {
            get { return ShellThumbnailRetrievalOption.Default; }
        }

        //public void FetchProperties(IItem item)
        //{
        //    if (item == null) return;

        //    if (_shellError)
        //    {
        //        SetPropertiesFromFileInfo(item);
        //    }
        //    else
        //    {
        //        try
        //        {
        //            item.ShellObject = ShellObject.FromParsingName(item.FullPath);
        //            item.ShellObject.Thumbnail.FormatOption = ThumbnailFormatOption;
        //            item.ShellObject.Thumbnail.RetrievalOption = ThumbnailRetrievalOption;
        //            SetPropertiesFromShellObject(item);
        //        } catch
        //        {
        //            _shellError = true;
        //        } 
        //    }
        //    FireChanges(item);
        //}

        //private static void SetPropertiesFromShellObject(IItem item)
        //{
        //    item.Length = (long)(item.ShellObject.Properties.System.Size.Value ?? 0);
        //    item.Created = item.ShellObject.Properties.System.DateCreated.Value;
        //    item.LastModified = item.ShellObject.Properties.System.DateCreated.Value;
        //    item.Thumbnail = item.ShellObject.Thumbnail.MediumBitmapSource;
        //    item.Thumbnail.Freeze();
        //}

        //private static void SetPropertiesFromFileInfo(IItem item)
        //{
        //    FileSystemInfo fileInfo;
        //    try
        //    {
        //        if (item.Type == ItemType.Container)
        //        {
        //            fileInfo = new DirectoryInfo(item.FullPath);
        //            item.Length = 0;
        //        }
        //        else
        //        {
        //            var fi = new FileInfo(item.FullPath);
        //            fileInfo = fi;
        //            item.Length = fi.Length;
        //        }
        //        item.Created = fileInfo.CreationTime;
        //        item.LastModified = fileInfo.LastWriteTime;
        //    } catch {}
        //}

        //public static void FireChanges(IItem item)
        //{
        //    item.OnNotifyPropertyChanged("Created");
        //    item.OnNotifyPropertyChanged("LastModified");
        //    item.OnNotifyPropertyChanged("Length");
        //    item.OnNotifyPropertyChanged("Thumbnail");
        //}

        public virtual void DoAction(IItem item)
        {
            var casted = item as IVirtualizedPropertyProvider<FileProperties>;
            var CurrentItem = casted.Properties;
            if (CurrentItem == null)
            {
                return;
            }
            if (CurrentItem.ShellObject != null && CurrentItem.ShellObject.IsLink)
            {
                var properties = CurrentItem.ShellObject.Properties;
                var target = ((string)properties.GetProperty("System.Link.TargetParsingPath").ValueAsObject);
                if (Directory.Exists(target))
                {
                    MessageHost.ViewHandler.SetDirectory(target);
                    return;
                }
            }
            if (item.ItemType == ItemType.Container)
            {
                MessageHost.ViewHandler.SetDirectory(item.FullPath);
            }
            else
            {
                var start = new ProcessStartInfo(item.FullPath);
                Process.Start(start);
            }
        }

        private bool _shellError;
        private BackgroundWorker _currentWorker;
    }

}
