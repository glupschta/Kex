using Microsoft.WindowsAPICodePack.Shell;

namespace Kex.Common
{
    public class PathResolver
    {
        public static string Resolve(ShellObject shellObject, string fullPath)
        {
            if (shellObject == null)
                return fullPath;
            if (shellObject.IsLink)
            {
                var link = ShellLink.FromParsingName(fullPath);
                var shellPath = ((string)shellObject.Properties.GetProperty("System.ParsingPath").ValueAsObject);
               // var shellPath = ((string)shellObject.Properties.GetProperty("System.Link.TargetParsingPath").ValueAsObject);
                //Für .lnk Systemsteuerung/Verwaltung
                return shellPath == shellObject.Name ? fullPath : shellPath;
            }
            return fullPath;
        }
    }
}
