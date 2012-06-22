using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Kex.Common
{
    public class Utils
    {
        public static T TryFindParent<T>(DependencyObject current) where T : class
        {
            DependencyObject parent = VisualTreeHelper.GetParent(current);

            if (parent == null) return null;
            if (parent is T) return parent as T;
            return TryFindParent<T>(parent);
        }
    }
}
