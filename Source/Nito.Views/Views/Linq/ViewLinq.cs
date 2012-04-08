using System;
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
        /// Casts the elements in a source view to a specified type.
        /// </summary>
        /// <typeparam name="TResult">The type of element observed by the resulting view.</typeparam>
        /// <param name="source">The source view.</param>
        /// <returns>The result view.</returns>
        public static IView<TResult> Cast<TResult>(this IView source)
        {
            return (source as System.Collections.IList).View<TResult>();
        }

        /// <summary>
        /// Creates a projected view of the data.
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
        /// Creates a projected, flattened view of the data.
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
            return source.Select(item => collectionSelector(item).Select(collection => new { Item = item, Collection = collection })).Concat()
                .Select(x => resultSelector(x.Item, x.Collection));
        }

        // TODO: Where, OrderBy/ThenBy.
    }
}
