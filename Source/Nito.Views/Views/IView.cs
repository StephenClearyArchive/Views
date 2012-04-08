using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Views
{
    /// <summary>
    /// A view of data that provides fast random-access to elements. Any type implementing this interface must also implement <see cref="IList{T}"/>. Views do not necessarily store data; they may be "virtual" in the sense of creating the data elements on request.
    /// </summary>
    /// <typeparam name="T">The type of item observed by the view.</typeparam>
    public interface IView<T> : Linq.IView
    {
    }
}
