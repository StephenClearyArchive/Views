using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace Views.Util
{
    /// <summary>
    /// Provides a delegate-based implementation of a constant view.
    /// </summary>
    /// <typeparam name="T">The type of elements observed by the view.</typeparam>
    internal sealed class AnonymousConstantView<T> : ConstantViewBase<T>
    {
        /// <summary>
        /// The backing field for <see cref="Count"/>.
        /// </summary>
        private int count;

        /// <summary>
        /// The delegate used to get items in the list.
        /// </summary>
        public Func<int, T> GetItem { get; set; }

        /// <summary>
        /// Gets the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item to get.</param>
        public override T this[int index]
        {
            get
            {
                Contract.Assert(this.GetItem != null);
                return this.GetItem(index);
            }
        }

        /// <summary>
        /// Gets the number of elements observed by this view.
        /// </summary>
        /// <returns>The number of elements observed by this view.</returns>
        public override int Count
        {
            get { return this.count; }
        }

        /// <summary>
        /// Sets the number of elements observed by this view.
        /// </summary>
        public int SetCount
        {
            set
            {
                Contract.Requires(value >= 0);
                this.count = value;
            }
        }
    }
}
