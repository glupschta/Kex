using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kex.Model.ItemProvider;

namespace Kex.Model
{
    public interface IVirtualizedPropertyProvider<out T>
    {
        T Properties { get; }
        bool Loaded { get; set; }
    }
}
