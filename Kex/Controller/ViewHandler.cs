using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Kex.Model.ItemProvider;
using Kex.Views;
using Microsoft.WindowsAPICodePack.Shell;

namespace Kex.Controller
{
    public class ViewHandler
    {
        public const string FullView = "Full";
        public const string SimpleView = "Simple";
        public const string ThumbView = "Thumbs";

        public ViewHandler(ListerView listerView)
        {
            this.listerView = listerView;
        }

        public void SetView(string view)
        {
            var xamlView = listerView.FindResource(view) as ViewBase;
            if (xamlView == null)
                throw new Exception("View not found: " + view);
            
            listerView.View.View = xamlView;

            string itemTemplateName;
            var fileItemProvider = listerView.Lister.ItemProvider as FilesystemItemProvider;
            if (view == "fullView")
            {
                itemTemplateName = "gridVirtualizing";
                if (fileItemProvider != null)
                {
                    fileItemProvider.ThumbnailFormatOption = ShellThumbnailFormatOption.IconOnly;
                }
            }
            else
            {
                itemTemplateName = "tileVirtualizing";
                if (fileItemProvider != null)
                {
                    fileItemProvider.ThumbnailFormatOption = ShellThumbnailFormatOption.Default;
                }
            }

            var itemsPanel = (ItemsPanelTemplate)listerView.FindResource(itemTemplateName);
            if (itemsPanel == null)
                throw new Exception("ItemPanelTemplate not found for view: "+view);
            listerView.View.ItemsPanel = itemsPanel;
        }

        private readonly ListerView listerView;
    }
}
