using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace Views.Util
{
    /// <summary>
    /// Provides common implementations of some view methods.
    /// </summary>
    /// <typeparam name="T">The type of element observed by the view.</typeparam>
    public abstract class ViewBase<T> : IView<T>, IEnumerable<T>
    {
        /// <summary>
        /// Gets the number of elements observed by this view.
        /// </summary>
        /// <returns>The number of elements observed by this view.</returns>
        public abstract int Count { get; }

        /// <summary>
        /// Gets the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item to get.</param>
        public abstract T this[int index] { get; }

        public IEnumerator<T> GetEnumerator()
        {
            int count = this.Count;
            for (int i = 0; i != count; ++i)
                yield return this[i];
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
