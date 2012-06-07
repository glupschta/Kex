using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Kex.Common
{
    public class MenuHandler
    {
        private const string MenuConfigFile = "Menu.xml";

        private MenuHandler()
        {
            Initialize();
        }

        private static MenuHandler instance;
        public static MenuHandler Instance
        {
            get { return instance ?? (instance = new MenuHandler()); }
        }

        private void Initialize()
        {
            var executionPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);

            var config = XDocument.Load(Path.Combine(executionPath, MenuConfigFile));
            var menuRoot = config.Elements().First();
            allMenus = new List<MenuItem>();
            foreach (var menuElement in menuRoot.Elements())
            {
                var name = menuElement.Attributes().First(it => it.Name == "Text").Value;
                var parent = new MenuItem(null, name, null);
                HandleMenuItems(parent, menuElement);
                allMenus.Add(parent);
            }
        }

        public void HandleMenuItems(MenuItem parent, XElement xElement)
        {
            foreach (var menuItem in xElement.Elements())
            {
                var name = menuItem.Attributes().First(it => it.Name == "Text").Value;
                var commandAttribute = menuItem.Attributes().FirstOrDefault(it => it.Name == "Command");
                var command = commandAttribute != null ? commandAttribute.Value : string.Empty;

                var argumentAttribute = menuItem.Attributes().FirstOrDefault(at => at.Name == "Argument");
                var argument = argumentAttribute != null ? argumentAttribute.Value : string.Empty;

                var item = new MenuItem(parent, name, command, argument);
                HandleMenuItems(item, menuItem);
                parent.SubItems.Add(item);
            }
        }

        public MenuItem GetMenuByName(string name)
        {
            return allMenus.FirstOrDefault(m => m.Name == name);
        }

        private List<MenuItem> allMenus;
    }

}
