using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kex.Controller;
using Kex.Model.Item;

namespace Kex.Model.Lister
{
    public class PropertyLister : BaseLister
    {
        private readonly FileItem FileItem;

        public PropertyLister(FileItem fileItem) : base()
        {
            FileItem = fileItem;
        }

        public override void SelectionChanged(System.Windows.Controls.ListView view, System.Windows.Controls.SelectionChangedEventArgs selectionChangedEventArgs){
        
        }

        public override void Refresh()
        {
            if (FileItem == null) return;
            var items = new List<PropertyItem>();

            if (FileItem.FullPath.ToLower().EndsWith(".dll") || FileItem.FullPath.EndsWith(".exe"))
            {
                try
                {
                    var container = new PropertyItem("Cecil", "", ItemType.Container);
                    var types = new PropertyItem("Types", "", ItemType.Container);
                    var references = new PropertyItem("References", "", ItemType.Container);
                    var ressources = new PropertyItem("Ressources", "", ItemType.Container);
                    var a = Mono.Cecil.AssemblyDefinition.ReadAssembly(FileItem.FullPath);

                    container.Childs.Add(new PropertyItem("Name", a.MainModule.Assembly.FullName, ItemType.Executable));
                    container.Childs.Add(new PropertyItem("Architecture", a.MainModule.Architecture.ToString(), ItemType.Executable));
                    container.Childs.Add(new PropertyItem("Runtime", a.MainModule.Runtime.ToString(), ItemType.Executable));
                    foreach(var attr in a.CustomAttributes)
                    {
                        if (attr.HasConstructorArguments && !string.IsNullOrEmpty(attr.ConstructorArguments.First().Value.ToString()))
                        {
                            container.Childs.Add(new PropertyItem(attr.AttributeType.Name,
                                                                  attr.ConstructorArguments.First().Value.ToString(),
                                                                  ItemType.Executable));
                        }
                    }
                    container.Childs.Add(references);
                    container.Childs.Add(types);
                    container.Childs.Add(ressources);

                    
                    types.Childs.AddRange(a.MainModule.Types.Select(m => new PropertyItem("Type",m.FullName ,ItemType.Executable)));
                    references.Childs.AddRange(a.MainModule.AssemblyReferences.Select(ar => new PropertyItem("Reference", ar.FullName, ItemType.Executable)));
                    ressources.Childs.AddRange(a.MainModule.Resources.Select(ar => new PropertyItem("Ressource", ar.Name, ItemType.Executable)));
                    items.Add(container);
                }
                catch (Exception ex){ MainWindow.Debug(ex); }
            }

            items.AddRange(FileItem.ShellObject.Properties.DefaultPropertyCollection
                .Where(pc => pc.Description != null && pc.Description.DisplayName != null && pc.ValueAsObject != null)
                .Select(pc => new PropertyItem(pc.Description.DisplayName, pc.ValueAsObject.ToString(), ItemType.Executable)));

            Items = items;
        }

        public override string XamlView
        {
            get { return null; }
        }

        public override void DoAction(object obj)
        {
            var pi = obj as PropertyItem;
            if (pi == null) return;

            if (pi.ItemType == ItemType.Container)
            {
                Items = pi.Childs;
                ListerManager.Instance.CommandManager.CurrentView.View.Items.SortDescriptions.Clear();
                ListerManager.Instance.CommandManager.CurrentView.View.SelectedIndex = 0;
                ListerManager.Instance.CommandManager.UpdateColumnWidth();
                ListerManager.Instance.CommandManager.FocusView();
            }
        }

        private IEnumerable<Column> columns;
        public override IEnumerable<Column> Columns
        {
            get
            {
                return columns ?? (columns = new[]
                                                   {
                                                       new Column("Name", "Name"), 
                                                       new Column("Value", "Value")
                                                   });
            }
        }
    }

}
