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
    public class FilesystemItemProvider : IItemProvider<FileProperties>
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

        protected virtual IEnumerable<IItem<FileProperties>> GetItemsEnumerable()
        {
            var allItems = new List<IItem<FileProperties>>();
            allItems.Add(new FileItem(CurrentContainer+"\\..",ItemType.Container, this));
            allItems.AddRange(Directory.EnumerateDirectories(CurrentContainer).Select(di => new FileItem(di, ItemType.Container, this)));
            allItems.AddRange(Directory.EnumerateFiles(CurrentContainer).Select(fi => new FileItem(fi, ItemType.Executable, this)));
            return allItems;
        }

        public IEnumerable<IItem<FileProperties>> GetItems()
        {
            var allItems = GetItemsEnumerable();
            try
            {
                if (allItems.Any())
                {
                    const int preload = 1;
                    foreach(var item in allItems.Take(preload))
                    {
                        FetchDetails(item);
                    }
                    FetchPropertiesAsync(allItems.Skip(preload));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return allItems;
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
                props.Thumbnail = props.ShellObject.Thumbnail.MediumBitmapSource;
                props.Thumbnail.Freeze();
                item.Properties = props;
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

        protected ShellThumbnailFormatOption ThumbnailFormatOption { get; set; }

        protected ShellThumbnailRetrievalOption ThumbnailRetrievalOption { get; set; }

        public virtual void DoAction(IItem item)
        {
            var currentItem = ((IItem<FileProperties>)item).Properties;
            if (currentItem == null) return;
            if (currentItem.ShellObject != null && currentItem.ShellObject.IsLink)
            {
                var properties = currentItem.ShellObject.Properties;
                var target = ((string)properties.GetProperty("System.Link.TargetParsingPath").ValueAsObject);
                if (Directory.Exists(target))
                {
                    ListerManager.Instance.CommandManager.SetContainer(target);
                    return;
                }
            }
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
    }

}
