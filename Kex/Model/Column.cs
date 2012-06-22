using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kex.Model
{
    public class Column
    {
        public Column(string header, string bindingExpression)
        {
            Header = header;
            BindingExpression = bindingExpression;
        }

        public string Header { get; set; }
        public string BindingExpression { get; set; }
    }
}
