using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Views.Util;

namespace Views.Linq
{
    /// <summary>
    /// A non-generic interface for views. This is only necessary for LINQ support and is not intended for end-user code.
    /// </summary>
    public interface IView
    {
    }

    /// <summary>
    /// An ordered view. This is only necessary for LINQ support and is not intended for end-user code.
    /// </summary>
    /// <typeparam name="T">The type of item observed by the view.</typeparam>
    public interface IOrderedView<T> : IView<T>
    {
        /// <summary>
        /// Gets the comparer used to indirectly compare source list elements.
        /// </summary>
        IndirectComparer<T> IndexComparer { get; }
    }
}
