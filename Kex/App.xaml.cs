
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Kex.Common;
using Kex.Controller;
using FontFamily = System.Windows.Media.FontFamily;

namespace Kex
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_Startup(object sender, StartupEventArgs e)
        {
            this.Properties["StartupArguments"] = e.Args;

            TextElement.FontFamilyProperty.OverrideMetadata(
                typeof(TextElement),
                new FrameworkPropertyMetadata(Options.FontFamily));

            TextBlock.FontFamilyProperty.OverrideMetadata(
            typeof(TextBlock),
            new FrameworkPropertyMetadata(Options.FontFamily));

            TextElement.FontSizeProperty.OverrideMetadata(
                typeof(TextElement),
                new FrameworkPropertyMetadata(Options.FontSize));

            TextBlock.FontSizeProperty.OverrideMetadata(
            typeof(TextBlock),
            new FrameworkPropertyMetadata(Options.FontSize));
        }
    }
}
