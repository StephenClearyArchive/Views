using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Views.Linq
{
    /// <summary>
    /// A non-generic interface for views. This is only necessary for LINQ support and is not intended for end-user code. Types implmenting this interface must also implement <see cref="System.Collections.IList"/>.
    /// </summary>
    public interface IView
    {
    }
}
