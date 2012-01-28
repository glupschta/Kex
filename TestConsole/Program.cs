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
            string path = @"\\poing\mp3";
            var shares = NetworkListManager.GetNetworks(NetworkConnectivityLevels.All);
            foreach(var s in shares)
            {
                Console.WriteLine(s.Name);
            }
            ProcessStartInfo psi = new ProcessStartInfo("\\poing");
            Process.Start(psi);
            Console.ReadLine();
        }
    }
}
