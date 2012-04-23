using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Views.Util;
using Comparers;

namespace Views.Linq
{
    /// <summary>
    /// Provides extension methods for using views via LINQ.
    /// </summary>
    public static class ViewLinq
    {
        /// <summary>
        /// Casts the elements in a source view to a specified type. This method is for LINQ support and is not intended for end-user code (use <see cref="ViewExtensions.Map{TSource,TResult}"/> instead).
        /// </summary>
        /// <typeparam name="TResult">The type of element observed by the resulting view.</typeparam>
        /// <param name="source">The source view.</param>
        /// <returns>The result view.</returns>
        public static IView<TResult> Cast<TResult>(this IView source)
        {
            return new DynamicViewWrapper<TResult>(source);
        }

        /// <summary>
        /// Creates a filtered view of the data. This method is for LINQ support and is not intended for end-user code (use <see cref="ViewExtensions.Filter"/> instead).
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="source">The source view.</param>
        /// <param name="predicate">The filter method to apply on the data.</param>
        /// <returns>The filtered view.</returns>
        public static IView<T> Where<T>(this IView<T> source, Func<T, bool> predicate)
        {
            return source.Filter(predicate);
        }

        /// <summary>
        /// Creates a projected view of the data. This method is for LINQ support and is not intended for end-user code (use <see cref="ViewExtensions.Map{TSource,TResult}"/> instead).
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
        /// Creates a projected, flattened view of the data. This method is for LINQ support and is not intended for end-user code (use <see cref="ViewExtensions.Map{TSource,TResult}"/> and <see cref="O:ViewExtensions.Concat"/> instead).
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

        // TODO: Join

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
            return new SortedView<T>(source, Compare.Key<T>.OrderBy(selector));
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
            return new SortedView<T>(source, Compare.Key<T>.OrderByDescending(selector));
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
            return new SortedView<T>(source, source.IndexComparer.Source.ThenBy(Compare.Key<T>.OrderBy(selector)));
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
            return new SortedView<T>(source, source.IndexComparer.Source.ThenBy(Compare.Key<T>.OrderByDescending(selector)));
        }
    }
}
