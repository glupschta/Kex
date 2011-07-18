using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Kex.Common;
using Kex.Interfaces;
using Microsoft.WindowsAPICodePack.Shell;

namespace Kex.Modell.ItemProvider
{
    public class FilesystemItemProvider : IItemProvider
    {
        public FilesystemItemProvider() {}

        public FilesystemItemProvider(string directory)
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
            allItems.AddRange(Directory.EnumerateDirectories(CurrentContainer).Select(di => new Item(di, ItemType.Container)));
            allItems.AddRange(Directory.EnumerateFiles(CurrentContainer).Select(fi => new Item(fi, ItemType.Executable)));
            return allItems;
        }

        public IEnumerable<IItem> GetItems()
        {
            var allItems = GetItemsEnumerable();
            try
            {
                if (allItems.Any())
                {
                    //FetchProperties(allItems.First());
                    FetchPropertiesAsync(allItems);
                }
            }
            catch (Exception ex)
            {
                MessageHost.ViewHandler.HandleException(ex);
            }
            
            return allItems;
        }

        private static BackgroundWorker _currentWorker;

        [STAThread]
        public void FetchPropertiesAsync(IEnumerable<IItem> items)
        {
            if (_currentWorker != null)
            {
                _currentWorker.CancelAsync();
            }

            _currentWorker = new BackgroundWorker { WorkerSupportsCancellation = true };
            _currentWorker.DoWork += WorkerDoWork;
            _currentWorker.RunWorkerAsync(items);
            _currentWorker.RunWorkerCompleted += CurrentWorkerRunWorkerCompleted;
        }

        static void CurrentWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _currentWorker = null;
        }

        private void WorkerDoWork(object sender, DoWorkEventArgs e)
        {
            _shellError = false;
            var items = e.Argument as IEnumerable<IItem>; 
            if (items == null) return;
            foreach (var item in items)
            {
                FetchProperties(item);
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

        public void FetchProperties(IItem item)
        {
            if (item == null) return;

            if (_shellError)
            {
                SetPropertiesFromFileInfo(item);
            }
            else
            {
                try
                {
                    item.ShellObject = ShellObject.FromParsingName(item.FullPath);
                    item.ShellObject.Thumbnail.FormatOption = ThumbnailFormatOption;
                    item.ShellObject.Thumbnail.RetrievalOption = ThumbnailRetrievalOption;
                    SetPropertiesFromShellObject(item);
                } catch
                {
                    _shellError = true;
                } 
            }
            FireChanges(item);
        }

        private static void SetPropertiesFromShellObject(IItem item)
        {
            item.Length = (long)(item.ShellObject.Properties.System.Size.Value ?? 0);
            item.Created = item.ShellObject.Properties.System.DateCreated.Value;
            item.LastModified = item.ShellObject.Properties.System.DateCreated.Value;
            item.Thumbnail = item.ShellObject.Thumbnail.MediumBitmapSource;
            item.Thumbnail.Freeze();
        }

        private static void SetPropertiesFromFileInfo(IItem item)
        {
            FileSystemInfo fileInfo;
            try
            {
                if (item.Type == ItemType.Container)
                {
                    fileInfo = new DirectoryInfo(item.FullPath);
                    item.Length = 0;
                }
                else
                {
                    var fi = new FileInfo(item.FullPath);
                    fileInfo = fi;
                    item.Length = fi.Length;
                }
                item.Created = fileInfo.CreationTime;
                item.LastModified = fileInfo.LastWriteTime;
            } catch {}
        }

        public static void FireChanges(IItem item)
        {
            //item.OnNotifyPropertyChanged("ShellObject");
            item.OnNotifyPropertyChanged("Created");
            item.OnNotifyPropertyChanged("LastModified");
            item.OnNotifyPropertyChanged("Length");
            item.OnNotifyPropertyChanged("Thumbnail");
        }

        public virtual void DoAction(IItem item)
        {
            var CurrentItem = item;
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
            if (CurrentItem.Type == ItemType.Container)
            {
                MessageHost.ViewHandler.SetDirectory(CurrentItem.FullPath);
            }
            else
            {
                var start = new ProcessStartInfo(CurrentItem.ResolvedPath);
                Process.Start(start);
            }
        }

        private bool _shellError;
    }

}
