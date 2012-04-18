using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace Views.Util
{
    /// <summary>
    /// An "indirect comparer", which compares two index values by comparing their elements in a view.
    /// </summary>
    /// <typeparam name="T">The type of elements observed by the view.</typeparam>
    public sealed class IndirectComparer<T> : IComparer<int>
    {
        /// <summary>
        /// The source view.
        /// </summary>
        private readonly IView<T> view;

        /// <summary>
        /// The source element comparer.
        /// </summary>
        private readonly IComparer<T> comparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndirectComparer&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="view">The source view.</param>
        /// <param name="comparer">The source element comparer.</param>
        public IndirectComparer(IView<T> view, IComparer<T> comparer = null)
        {
            Contract.Requires(view != null);
            this.view = view;
            this.comparer = comparer ?? Comparer<T>.Default;
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.view != null);
            Contract.Invariant(this.comparer != null);
        }

        /// <summary>
        /// Gets the source view.
        /// </summary>
        public IView<T> View
        {
            get { return this.view; }
        }

        /// <summary>
        /// Gets the source element comparer.
        /// </summary>
        public IComparer<T> Comparer
        {
            get { return this.comparer; }
        }

        /// <summary>
        /// Compares the index values by comparing their elements in the soruce view.
        /// </summary>
        /// <param name="x">The first index value.</param>
        /// <param name="y">The second index value.</param>
        /// <returns>A value less than zero if <paramref name="x"/> &lt; <paramref name="y"/>; zero if <paramref name="x"/> == <paramref name="y"/>; or a value greater than zero if <paramref name="x"/> &gt; <paramref name="y"/>.</returns>
        public int Compare(int x, int y)
        {
            return this.comparer.Compare(this.view[x], this.view[y]);
        }
    }
}
