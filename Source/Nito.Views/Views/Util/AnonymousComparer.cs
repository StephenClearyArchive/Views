﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Views.Util
{
    /// <summary>
    /// An object that implements <see cref="IComparer{T}"/> using a delegate. <see cref="Compare"/> must be set before using an instance of this class.
    /// </summary>
    /// <typeparam name="T">The type of items to compare.</typeparam>
    public sealed class AnonymousComparer<T> : IComparer<T>
    {
        /// <summary>
        /// Gets or sets the Compare delegate, which compares two objects and returns a value less than 0 if its first argument is less than its second argument, 0 if its two arguments are equal, or greater than 0 if its first argument is greater than its second argument.
        /// </summary>
        public Func<T, T, int> Compare { get; set; }

        /// <summary>
        /// Compares two objects and returns a value less than 0 if <paramref name="x"/> is less than <paramref name="y"/>, 0 if <paramref name="x"/> is equal to <paramref name="y"/>, or greater than 0 if <paramref name="x"/> is greater than <paramref name="y"/>.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>A value less than 0 if <paramref name="x"/> is less than <paramref name="y"/>, 0 if <paramref name="x"/> is equal to <paramref name="y"/>, or greater than 0 if <paramref name="x"/> is greater than <paramref name="y"/>.</returns>
        int IComparer<T>.Compare(T x, T y)
        {
            return this.Compare(x, y);
        }
    }
}
