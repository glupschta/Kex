using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kex.Model
{
    public interface IItemActions
    {
        void Copy();
        void Move();
        void ShowProperties();
        void DoAction();
    }
}
