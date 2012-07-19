using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows;
using Kex.Controller;
using Kex.Model;
using Kex.Model;
using Kex.Model.Item;
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
            if (targetLocation== null) return null;

            var selection = Clipboard.GetFileDropList();
            string target = null;
            foreach(var filePath in selection)
            {
                try
                {
                    if (Directory.Exists(filePath))
                    {
                        target = getTargetLocation(targetLocation, filePath, true);
                        if (ActionType == FileActionType.Copy)
                            FileSystem.CopyDirectory(filePath, target);
                        else if (ActionType == FileActionType.Move)
                            FileSystem.MoveDirectory(filePath, target);
                    }
                    else if (File.Exists(filePath))
                    {
                        target = getTargetLocation(targetLocation, filePath, false);
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

        private static string getTargetLocation(string targetLocation, string fileName, bool isDirectory)
        {
            var target = Path.Combine(targetLocation, fileName);
            while (isDirectory ? Directory.Exists(target) : File.Exists(target))
            {
                var finf = new FileInfo(target);
                var temp = finf.Name.Substring(0, finf.Name.Length - finf.Extension.Length);
                if (temp.Last() == ')')
                {
                    var fileIndex = 0;
                    var currentIndex = temp.Length - 1;
                    var multiplier = 1;
                    while(Char.IsDigit(temp[--currentIndex]))
                    {
                        fileIndex = fileIndex  + (temp[currentIndex] - '0') * multiplier;
                        multiplier *= 10;
                    }
                    target = temp[currentIndex] == '(' 
                        ? Path.Combine(targetLocation, string.Format("{0}({1}){2}", temp.Substring(0, currentIndex), ++fileIndex, finf.Extension)) 
                        : Path.Combine(targetLocation, temp + "(1)" + finf.Extension);
                }
                else
                {
                    target = Path.Combine(targetLocation, temp+"(1)"+finf.Extension);
                }
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
