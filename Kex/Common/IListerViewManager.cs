using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kex.Model;
using Kex.Model.Lister;
using Kex.Views;

namespace Kex.Common
{
    public interface IListerViewManager
    {
        ListerView CurrentListerView { get; set; }
        ListboxTextInput TextInput { get; }
        void OpenLister(string directory);
        void OpenLister(string container, ILister lister);
        void CloseCurrentLister();
        void CycleListers(int direction);
        void SetHeader(string directory);
        List<ILister> Listers { get;  }
    }
}
