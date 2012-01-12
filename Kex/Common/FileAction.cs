using System;
using System.IO;
using Kex.Controller;
using Kex.Model;
using Kex.Modell;
using Microsoft.VisualBasic.FileIO;

namespace Kex.Common
{

    public class FileAction
    {
        public static string Do(ItemSelection selection, string targetLocation)
        {
            if (selection == null || targetLocation== null)
            {
                return null;
            }
            string target = null;
            foreach(var item in selection.Selection)
            {
                target = getTargetLocation(targetLocation, item);
                try
                {
                    if (item.ItemType == ItemType.Container)
                    {
                        if (selection.FileAction == FileActionType.Copy)
                            FileSystem.CopyDirectory(item.FullPath, target);
                        else if (selection.FileAction == FileActionType.Move)
                            FileSystem.MoveDirectory(item.FullPath, target);
                    }
                    else if (item.ItemType == ItemType.Executable)
                    {
                        if (selection.FileAction == FileActionType.Copy)
                            File.Copy(item.FullPath, target);
                        else if (selection.FileAction == FileActionType.Move)
                            File.Move(item.FullPath, target);
                    }
                }
                catch (Exception ex)
                {
                    ListerManager.Instance.CommandManager.HandleException(ex);
                }
            }
            return target;
        }

        private static string getTargetLocation(string targetLocation, IItem item)
        {
            var temp = Path.Combine(targetLocation, item.Name);
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

        public static void Delete(ItemSelection selection)
        {
            foreach (var item in selection.Selection)
            {
                try
                {
                    switch (item.ItemType)
                    {
                        case ItemType.Container:
                            FileSystem.DeleteDirectory(item.FullPath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                            break;
                        case ItemType.Executable:
                            File.Delete(item.FullPath);
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
