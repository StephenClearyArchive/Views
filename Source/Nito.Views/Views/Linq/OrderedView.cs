﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Views.Util;

namespace Views.Linq
{
    /// <summary>
    /// A LINQ-compatible sorted list, which provides a sorted view into a source list.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    internal sealed class OrderedView<T> : SortedList<T>, IOrderedView<T>
    {
        /// <summary>
        /// The comparer used to compare source list elements.
        /// </summary>
        private readonly IComparer<T> comparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedView&lt;T&gt;"/> class for the given source list.
        /// </summary>
        /// <param name="source">The source list.</param>
        /// <param name="comparer">The source list element comparer. If this is <c>null</c>, then <see cref="Comparer{T}.Default"/> is used.</param>
        public OrderedView(IList<T> source, IComparer<T> comparer)
            : base(source, comparer)
        {
            this.comparer = comparer ?? Comparer<T>.Default;
        }

        IComparer<T> IOrderedView<T>.Comparer
        {
            get { return this.comparer; }
        }
    }
}