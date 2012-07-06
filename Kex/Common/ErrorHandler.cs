using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Kex.Common
{
    public static class ErrorHandler
    {
        public static void ShowError(Exception ex)
        {
            ShowError(ex.ToString());
        }

        public static void ShowError(string message)
        {
            MessageBox.Show(message, "Error");
        }
    }
}
