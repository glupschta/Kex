using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Kex.Common
{
    public static class Options
    {

        private static readonly AppSettingsReader settings;
        static Options()
        {
            settings = new AppSettingsReader();  
        }

        public static FontFamily FontFamily
        {
            get { return  new FontFamily((string)settings.GetValue("FontFamily", typeof (string))); }
        }

        public static double FontSize
        {
            get { return double.Parse((string)settings.GetValue("FontSize", typeof(string))); }
        }
    }
}
