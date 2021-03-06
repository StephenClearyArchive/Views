﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace Views
{
    /// <summary>
    /// Provides sources for programmatic views.
    /// </summary>
    public static class View
    {
        /// <summary>
        /// A type that caches instances of constant, empty views.
        /// </summary>
        /// <typeparam name="T">The type of elements observed by the view.</typeparam>
        private static class EmptyInstance<T>
        {
            /// <summary>
            /// Static constructor to force lazy initialization of this type; see
            ///  https://msmvps.com/blogs/jon_skeet/archive/2010/01/26/type-initialization-changes-in-net-4-0.aspx
            /// and
            ///  http://msmvps.com/blogs/jon_skeet/archive/2012/04/07/type-initializer-circular-dependencies.aspx
            /// </summary>
            static EmptyInstance()
            {
            }

            /// <summary>
            /// The constant, empty view.
            /// </summary>
            public static readonly IView<T> Instance = new Util.AnonymousConstantView<T> { SetCount = 0 };
        }

        /// <summary>
        /// Returns a constant view that generates its elements when they are observed, passing the element's index to the generator delegate.
        /// </summary>
        /// <typeparam name="T">The type of elements observed by the view.</typeparam>
        /// <param name="generator">The delegate that is used to generate the elements. May not be <c>null</c>.</param>
        /// <param name="count">The number of elements observed by the view. This must be greater than or equal to <c>0</c>.</param>
        /// <returns>A constant view that generates its elements on demand.</returns>
        public static IView<T> Generate<T>(Func<int, T> generator, int count)
        {
            Contract.Requires(generator != null);
            Contract.Requires(count >= 0);
            Contract.Ensures(Contract.Result<IView<T>>() != null);
            return new Util.AnonymousConstantView<T>
            {
                SetCount = count,
                GetItem = generator,
            };
        }

        /// <summary>
        /// Returns a constant view that generates numbers in a sequence.
        /// </summary>
        /// <param name="start">The value at which the view begins (inclusive).</param>
        /// <param name="count">The number of elements observed by the view. This must be greater than or equal to <c>0</c>.</param>
        /// <param name="step">The stride of the view. Defaults to <c>1</c>.</param>
        /// <returns>A read-only view that generates numbers in a sequence.</returns>
        public static IView<int> Range(int start, int count, int step = 1)
        {
            Contract.Requires(count >= 0);
            Contract.Ensures(Contract.Result<IView<int>>() != null);
            if (count == 0)
                return EmptyInstance<int>.Instance;
            return Generate<int>(i => start + i * step, count);
        }

        /// <summary>
        /// Returns an empty, constant view. If you want a mutable empty view, use <c>new List&lt;T&gt;.View()</c> or <c>new ObservableCollection&lt;T&gt;.View()</c>.
        /// </summary>
        /// <typeparam name="T">The type of elements observed by the view.</typeparam>
        /// <returns>An empty, constant view.</returns>
        public static IView<T> Empty<T>()
        {
            Contract.Ensures(Contract.Result<IView<T>>() != null);
            return EmptyInstance<T>.Instance;
        }

        /// <summary>
        /// Converts a single value into a constant view which observes that value the specified number of times.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="source">The value.</param>
        /// <param name="count">The number of times <paramref name="source"/> is repeated. If <paramref name="count"/> is 0, an empty view is returned. This must be greater than or equal to <c>0</c>.</param>
        /// <returns>A constant view observing <paramref name="count"/> elements, all equal to <paramref name="source"/>.</returns>
        public static IView<T> Repeat<T>(T source, int count)
        {
            Contract.Requires(count >= 0);
            Contract.Ensures(Contract.Result<IView<T>>() != null);
            if (count == 0)
                return EmptyInstance<T>.Instance;
            return Generate<T>(_ => source, count);
        }

        /// <summary>
        /// Converts values into a constant view which observes those values.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="sources">The values.</param>
        /// <returns>A constant view observing the given values.</returns>
        public static IView<T> Return<T>(params T[] sources)
        {
            Contract.Requires(sources != null);
            Contract.Ensures(Contract.Result<IView<T>>() != null);
            return Generate<T>(i => sources[i], sources.Length);
        }
    }
}
