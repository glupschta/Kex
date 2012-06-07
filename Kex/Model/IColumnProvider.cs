using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kex.Model
{
    public interface IColumnProvider
    {
        Dictionary<string, string> Columns { get; }
    }
}
