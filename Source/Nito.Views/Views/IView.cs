using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Views
{
    public interface IView<T>
    {
        IList<T> AsList();
    }
}
