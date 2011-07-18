using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Shell32;
using Kex.Interfaces;

namespace Kex
{
    public class WindowsFunctions
    {
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        static extern bool ShellExecuteEx(ref Shellexecuteinfo lpExecInfo);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct Shellexecuteinfo
        {
            public int cbSize;
            public uint fMask;
            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpVerb;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpFile;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpParameters;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpDirectory;
            public int nShow;
            public IntPtr hInstApp;
            public IntPtr lpIDList;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpClass;
            public IntPtr hkeyClass;
            public uint dwHotKey;
            public IntPtr hIcon;
            public IntPtr hProcess;
        }

        private const int SW_SHOW = 5;
        private const uint SEE_MASK_INVOKEIDLIST = 12;
        public static void ShowFileProperties(string filename)
        {
            var info = new Shellexecuteinfo();
            info.lpFile = filename;
            info.lpVerb = "properties";
            info.cbSize = Marshal.SizeOf(info);
            info.nShow = SW_SHOW;
            info.fMask = SEE_MASK_INVOKEIDLIST;
            ShellExecuteEx(ref info);
        }

    }
}
