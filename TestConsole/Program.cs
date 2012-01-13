using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"\\poing\mp3";
            var shares1 = new NetWorkShare().GetShares("poing");
            ShellObject 
            var shares = Directory.EnumerateDirectories(path);
            foreach(var s in shares)
            {
                Console.WriteLine(s);
            }
            Console.ReadLine();
        }
    }
}
