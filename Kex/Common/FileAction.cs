using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows;
using Kex.Controller;
using Kex.Model;
using Kex.Model;
using Microsoft.VisualBasic.FileIO;

namespace Kex.Common
{

    public class FileAction
    {
        public static FileActionType ActionType { get; set; }

        public static void SetSelection()
        {
            var selection = ListerManager.Instance.ListerViewManager.CurrentListerView.Lister.Selection.Select(s => s.FullPath).ToArray();
            var files = new StringCollection();
            files.AddRange(selection);
            Clipboard.SetFileDropList(files);
        }

        public static string Paste(string targetLocation)
        {
            if (targetLocation== null)
            {
                return null;
            }
            var selection = Clipboard.GetFileDropList();
            string target = null;
            foreach(var filePath in selection)
            {
                target = getTargetLocation(targetLocation, filePath);
                try
                {
                    if (Directory.Exists(filePath))
                    {
                        if (ActionType == FileActionType.Copy)
                            FileSystem.CopyDirectory(filePath, target);
                        else if (ActionType == FileActionType.Move)
                            FileSystem.MoveDirectory(filePath, target);
                    }
                    else if (File.Exists(filePath))
                    {
                        if (ActionType == FileActionType.Copy)
                            File.Copy(filePath, target);
                        else if (ActionType == FileActionType.Move)
                            File.Move(filePath, target);
                    }
                }
                catch (Exception ex)
                {
                    ListerManager.Instance.CommandManager.HandleException(ex);
                }
            }
            return target;
        }

        private static string getTargetLocation(string targetLocation, string fileName)
        {
            var temp = Path.Combine(targetLocation, fileName);
            var target = temp;
            int i = 1;
            while (File.Exists(target))
            {
                var finf = new FileInfo(temp);
                var name = finf.FullName.Substring(0, finf.FullName.Length - finf.Extension.Length);
                target = String.Format("{0}({1}){2}", name, i++, finf.Extension);
            }
            return target;
        }

        public static void Delete()
        {

            var selection = ListerManager.Instance.ListerViewManager.CurrentListerView.Lister.Selection;
            foreach (var item in selection)
            {
                try
                {
                    var fi = item as FileItem;
                    if (fi == null) continue;

                    var pathToDelete = fi.ShellObject.IsLink ? fi.ShellObject.Properties.System.ParsingPath.Value : fi.FullPath;
                    switch (item.ItemType)
                    {
                        case ItemType.Container:
                            FileSystem.DeleteDirectory(pathToDelete, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                            break;
                        case ItemType.Executable:
                            File.Delete(pathToDelete);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ListerManager.Instance.CommandManager.HandleException(ex);
                }
            }
        }

    }

    public enum FileActionType
    {
        Move,
        Copy
    }

}
