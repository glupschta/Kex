using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.WindowsAPICodePack.Net;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            ShowFiles();
            Console.ReadLine();
        }

        static void ShowShares()
        {
            string path = @"\\poing\mp3";
            var shares = NetworkListManager.GetNetworks(NetworkConnectivityLevels.All);
            foreach (var s in shares)
            {
                Console.WriteLine(s.Name);
            }
            ProcessStartInfo psi = new ProcessStartInfo("\\poing");
            Process.Start(psi);
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
