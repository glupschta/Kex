using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Kex.Interfaces;
using Kex.Modell;
using Microsoft.VisualBasic.FileIO;

namespace Kex
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
                    if (item.Type == ItemType.Container)
                    {
                        if (selection.FileAction == FileActionType.Copy)
                            FileSystem.CopyDirectory(item.FullPath, target);
                        else if (selection.FileAction == FileActionType.Move)
                            FileSystem.MoveDirectory(item.FullPath, target);
                    }
                    else if (item.Type == ItemType.Executable)
                    {
                        if (selection.FileAction == FileActionType.Copy)
                            File.Copy(item.FullPath, target);
                        else if (selection.FileAction == FileActionType.Move)
                            File.Move(item.FullPath, target);
                    }
                }
                catch (Exception ex)
                {
                    MessageHost.ViewHandler.HandleException(ex);
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
                    if (item.Type == ItemType.Container)
                        FileSystem.DeleteDirectory(item.FullPath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                    else if (item.Type == ItemType.Executable)
                        File.Delete(item.FullPath);
                }
                catch (Exception ex)
                {
                    MessageHost.ViewHandler.HandleException(ex);
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
