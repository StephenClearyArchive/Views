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
    public sealed class IndirectComparer<T> : Comparers.Util.SourceComparerBase<int, T>
    {
        /// <summary>
        /// The source view.
        /// </summary>
        private readonly IView<T> view;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndirectComparer&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="view">The source view.</param>
        /// <param name="comparer">The source element comparer.</param>
        public IndirectComparer(IView<T> view, IComparer<T> comparer = null)
            : base(comparer, false)
        {
            Contract.Requires(view != null);
            this.view = view;
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.view != null);
        }

        /// <summary>
        /// Gets the source view.
        /// </summary>
        public IView<T> View
        {
            get { return this.view; }
        }

        /// <summary>
        /// Compares two objects and returns a value less than 0 if <paramref name="x"/> is less than <paramref name="y"/>, 0 if <paramref name="x"/> is equal to <paramref name="y"/>, or greater than 0 if <paramref name="x"/> is greater than <paramref name="y"/>.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>A value less than 0 if <paramref name="x"/> is less than <paramref name="y"/>, 0 if <paramref name="x"/> is equal to <paramref name="y"/>, or greater than 0 if <paramref name="x"/> is greater than <paramref name="y"/>.</returns>
        protected override int DoCompare(int x, int y)
        {
            Contract.Assume(x >= 0 && x < this.view.Count);
            Contract.Assume(y >= 0 && y < this.view.Count);
            return this.Source.Compare(this.view[x], this.view[y]);
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj">The object for which to return a hash code.</param>
        /// <returns>A hash code for the specified object.</returns>
        protected override int DoGetHashCode(int obj)
        {
            Contract.Assume(obj >= 0 && obj < this.view.Count);
            return Comparers.Util.ComparerHelpers.GetHashCodeFromComparer(this.Source, this.view[obj]);
        }
    }
}
