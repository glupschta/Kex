using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace Kex.Common
{
    public interface IPopupItem
    {
        string DisplayName { get; }
        string FilterString { get; }
        BitmapSource Thumbnail { get; }
    }
}
