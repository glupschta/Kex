using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using Kex.Common;
using Kex.Controller;
using Kex.Model;
using Kex.Model.Item;
using Microsoft.WindowsAPICodePack.Shell;

namespace Kex.Model.ItemProvider
{
    public class FilesystemItemProvider
    {
        public IEnumerable<FileItem> GetItems(string container)
        {
            var items = new List<FileItem>();
            items.AddRange(
                Directory.EnumerateDirectories(container)
                    .Select(di => GetItem(di, ItemType.Container))
                    .Where(item => item != null)
            );
            items.AddRange(
                    Directory.EnumerateFiles(container)
                        .Select(di => GetItem(di, ItemType.Executable))
                        .Where(item => item != null)
                );
            
            if (!items.Any())
            {
                items.Add(new FileItem(container + "\\..", ItemType.Container, this));
            }
            return items;


            //var itemsFound = false;

            //foreach (var item in
            //    Directory.EnumerateDirectories(container)
            //    .Select(di => GetItem(di, ItemType.Container))
            //    .Where(item => item != null))
            //{
            //    itemsFound = true;
            //    yield return item;
            //}

            //foreach (var item in
            //    Directory.EnumerateFiles(container)
            //    .Select(fi => GetItem(fi, ItemType.Executable))
            //    .Where(item => item != null))
            //{
            //    itemsFound = true;
            //    yield return item;
            //}

            //if (!itemsFound)
            //{
            //    yield return new FileItem(container + "\\..", ItemType.Container, this);
            //}
        }

        private FileItem GetItem(string path, ItemType type)
        {
            if (type == ItemType.Container)
            {
                var di = new DirectoryInfo(path);
                return (!ShowHiddenItems && (di.Attributes & FileAttributes.Hidden) != 0)
                    ? null
                    : new FileItem(path, type, this);
            }

            var fi = new FileInfo(path);
            return (!ShowHiddenItems && (fi.Attributes & FileAttributes.Hidden) != 0)
                ? null
                : new FileItem(path, type, this);
        }

        public bool ShowHiddenItems { get; set; }

      
    }

}
