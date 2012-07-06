using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
using System.Text;
using Kex.Model;
using Microsoft.WindowsAPICodePack.Net;
using Microsoft.WindowsAPICodePack.Shell;
using Mono.Cecil;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //testZip();
            //var l = Directory.EnumerateFiles(@"c:/documents and settings");
            //TestWatcher();
            testManifest();
            Console.ReadLine();
        }

        private static void testManifest()
        {
            var asmPath = @"C:\tfs\MH\Source\noe.MH.Forms\bin\noe.core.web.dll";
            var a = Mono.Cecil.AssemblyDefinition.ReadAssembly(asmPath);

            Console.WriteLine(a.FullName);
            foreach(var ar in a.MainModule.AssemblyReferences)
            {
                Console.WriteLine(ar.FullName);
            }
            

        }

        private static void testZip()
        {
        }

        private static void TestWatcher()
        {
            ShellObject so = ShellObject.FromParsingName("C:");
            ShellObjectWatcher sw = new ShellObjectWatcher(so, true);
            sw.ItemCreated += new EventHandler<ShellObjectChangedEventArgs>(watcher_ItemCreated);
            sw.ItemDeleted += new EventHandler<ShellObjectChangedEventArgs>(sw_ItemDeleted);
            sw.ItemRenamed += new EventHandler<ShellObjectRenamedEventArgs>(sw_ItemRenamed);
            sw.DirectoryCreated += new EventHandler<ShellObjectChangedEventArgs>(sw_DirectoryCreated);
            sw.DirectoryDeleted += new EventHandler<ShellObjectChangedEventArgs>(sw_DirectoryDeleted);
            sw.DirectoryRenamed += new EventHandler<ShellObjectRenamedEventArgs>(sw_DirectoryRenamed);
            sw.DirectoryUpdated += new EventHandler<ShellObjectChangedEventArgs>(sw_DirectoryUpdated);
            sw.Start();
            Console.ReadLine();
            sw.Stop();
            Console.WriteLine("End");
        }

        static void sw_DirectoryUpdated(object sender, ShellObjectChangedEventArgs e)
        {
            Console.WriteLine(e.ChangeType+" _ "+e.Path);
        }

        static void sw_DirectoryRenamed(object sender, ShellObjectRenamedEventArgs e)
        {
            Console.WriteLine(e.ChangeType + " _ " + e.Path + e.NewPath);

        }

        static void sw_DirectoryDeleted(object sender, ShellObjectChangedEventArgs e)
        {
            Console.WriteLine(e.ChangeType + " _ " + e.Path);

        }

        static void sw_DirectoryCreated(object sender, ShellObjectChangedEventArgs e)
        {
            Console.WriteLine(e.ChangeType + " _ " + e.Path);

        }

        static void sw_ItemRenamed(object sender, ShellObjectRenamedEventArgs e)
        {
            Console.WriteLine(e.ChangeType + " _ " + e.Path);

        }

        static void sw_ItemDeleted(object sender, ShellObjectChangedEventArgs e)
        {
            Console.WriteLine(e.ChangeType + " _ " + e.Path);

        }

        static void watcher_ItemCreated(object sender, ShellObjectChangedEventArgs e)
        {
           Console.WriteLine("Created "+e.Path); 
        }

        static void ShowShares()
        {
            var nws = new NetworkBrowser();
            var shares = nws.GetNetworkComputers();
            foreach (var s in shares)
            {
                Console.WriteLine(s);
            }
        }

        static void ShowFiles()
        {
            var path = @"C:\Windows\System32\drivers\";
            foreach (var fi in Directory.GetDirectories(path))
            {
                Console.WriteLine(fi);
            }
            foreach(var fi in Directory.EnumerateFiles(path))
            {
                Console.WriteLine(fi);
            }
        }
    }
}
