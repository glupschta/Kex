﻿using System.IO;

namespace Kex.Common
{
    public class PathPart
    {
        public PathPart(string directory)
        {
            var dirInfo = new DirectoryInfo(directory);
            this.Name = dirInfo.Name;
            this.FullPath = dirInfo.FullName;
        }

        public PathPart(string name, string fullPath)
        {
            this.Name = name;
            this.FullPath = fullPath;
        }

        public string Name { get; set; }
        public string FullPath { get; set; }
    }
}