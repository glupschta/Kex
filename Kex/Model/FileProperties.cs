using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using Kex.Model.ItemProvider;
using Microsoft.WindowsAPICodePack.Shell;

namespace Kex.Model
{
    public class FileProperties: IDisposable
    {
        public DateTime? LastModified { get; set; }
        public DateTime? Created { get; set; }
        public long Length { get; set; }
        public ShellObject ShellObject { get; set; }
        public BitmapSource Thumbnail { get; set; }
        
        public void Dispose()
        {
            Thumbnail = null;
            ShellObject.Dispose();
        }
    }
}
