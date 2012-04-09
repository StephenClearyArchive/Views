﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Views.Linq
{
    /// <summary>
    /// Provides extension methods for using views via LINQ.
    /// </summary>
    public static class ViewLinq
    {
        /// <summary>
        /// Casts the elements in a source view to a specified type. This method is for LINQ support and is not intended for end-user code (use <see cref="ViewExtensions.Map"/> instead).
        /// </summary>
        /// <typeparam name="TResult">The type of element observed by the resulting view.</typeparam>
        /// <param name="source">The source view.</param>
        /// <returns>The result view.</returns>
        public static IView<TResult> Cast<TResult>(this IView source)
        {
            return (source as System.Collections.IList).View<TResult>();
        }

        /// <summary>
        /// Creates a projected view of the data. This method is for LINQ support and is not intended for end-user code (use <see cref="ViewExtensions.Map"/> instead).
        /// </summary>
        /// <typeparam name="TSource">The type of element observed by the source view.</typeparam>
        /// <typeparam name="TResult">The type of element observed by the projected view.</typeparam>
        /// <param name="source">The source view.</param>
        /// <param name="read">The projection used when reading elements.</param>
        /// <returns>The projected view.</returns>
        public static IView<TResult> Select<TSource, TResult>(this IView<TSource> source, Func<TSource, TResult> read)
        {
            return source.Map(read);
        }

        /// <summary>
        /// Creates a projected, flattened view of the data. This method is for LINQ support and is not intended for end-user code (use <see cref="ViewExtensions.Map"/> and <see cref="ViewExtensions.Concat"/> instead).
        /// </summary>
        /// <typeparam name="TSource">The type of element observed by the source view.</typeparam>
        /// <typeparam name="TCollection">The type of element observed by the intermediate view.</typeparam>
        /// <typeparam name="TResult">The type of element observed by the projected view.</typeparam>
        /// <param name="source">The source view.</param>
        /// <param name="collectionSelector">The projection from the source data to the intermediate elements.</param>
        /// <param name="resultSelector">The projection from the source and intermediate data to the elements in the final view.</param>
        /// <returns>The projected, flattened view.</returns>
        public static IView<TResult> SelectMany<TSource, TCollection, TResult>(this IView<TSource> source, Func<TSource, IView<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        {
            return source.Map(item => collectionSelector(item).Map(collection => new { Item = item, Collection = collection })).Concat()
                .Map(x => resultSelector(x.Item, x.Collection));
        }

        /// <summary>
        /// Creates a sorted view of the data. This method is for LINQ support and is not intended for end-user code (use <see cref="ViewExtensions.Sort"/> instead).
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <typeparam name="TKey">The type of the sort key.</typeparam>
        /// <param name="source">The source view.</param>
        /// <param name="selector">The projection from element to key.</param>
        /// <returns>The sorted view.</returns>
        public static IOrderedView<T> OrderBy<T, TKey>(this IView<T> source, Func<T, TKey> selector)
        {
            return source.Sort(new Util.AnonymousComparer<T> { Compare = (x, y) => Comparer<TKey>.Default.Compare(selector(x), selector(y)) }) as IOrderedView<T>;
        }

        /// <summary>
        /// Creates a sorted view of the data. This method is for LINQ support and is not intended for end-user code (use <see cref="ViewExtensions.Sort"/> instead).
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <typeparam name="TKey">The type of the sort key.</typeparam>
        /// <param name="source">The source view.</param>
        /// <param name="selector">The projection from element to key.</param>
        /// <returns>The sorted view.</returns>
        public static IOrderedView<T> OrderByDescending<T, TKey>(this IView<T> source, Func<T, TKey> selector)
        {
            return source.Sort(new Util.AnonymousComparer<T> { Compare = (x, y) => Comparer<TKey>.Default.Compare(selector(y), selector(x)) }) as IOrderedView<T>;
        }

        /// <summary>
        /// Creates a more sorted view of the data. This method is for LINQ support and is not intended for end-user code (use <see cref="ViewExtensions.Sort"/> instead).
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <typeparam name="TKey">The type of the sort key.</typeparam>
        /// <param name="source">The source view.</param>
        /// <param name="selector">The projection from element to key.</param>
        /// <returns>The sorted view.</returns>
        public static IOrderedView<T> ThenBy<T, TKey>(this IOrderedView<T> source, Func<T, TKey> selector)
        {
            return source.Sort(new Util.AnonymousComparer<T>
            {
                Compare = (x, y) =>
                {
                    var primarySortResult = source.Comparer.Compare(x, y);
                    if (primarySortResult != 0)
                        return primarySortResult;
                    return Comparer<TKey>.Default.Compare(selector(x), selector(y));
                }
            }) as IOrderedView<T>;
        }

        /// <summary>
        /// Creates a more sorted view of the data. This method is for LINQ support and is not intended for end-user code (use <see cref="ViewExtensions.Sort"/> instead).
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <typeparam name="TKey">The type of the sort key.</typeparam>
        /// <param name="source">The source view.</param>
        /// <param name="selector">The projection from element to key.</param>
        /// <returns>The sorted view.</returns>
        public static IOrderedView<T> ThenByDescending<T, TKey>(this IOrderedView<T> source, Func<T, TKey> selector)
        {
            return source.Sort(new Util.AnonymousComparer<T>
            {
                Compare = (x, y) =>
                {
                    var primarySortResult = source.Comparer.Compare(x, y);
                    if (primarySortResult != 0)
                        return primarySortResult;
                    return Comparer<TKey>.Default.Compare(selector(y), selector(x));
                }
            }) as IOrderedView<T>;
        }

        // TODO: Where, Join, GroupJoin, GroupBy.
    }
}