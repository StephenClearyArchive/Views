using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// TODO: check parameters in all source and linq methods.
namespace Views
{
    /// <summary>
    /// Provides sources for programmatic views.
    /// </summary>
    public static class View
    {
        /// <summary>
        /// Returns a read-only view that generates its elements when they are observed, passing the element's index to the generator delegate.
        /// </summary>
        /// <typeparam name="T">The type of elements observed by the view.</typeparam>
        /// <param name="generator">The delegate that is used to generate the elements.</param>
        /// <param name="count">The delegate that returns the number of elements observed by the view.</param>
        /// <returns>A read-only view that generates its elements on demand.</returns>
        public static IView<T> Generate<T>(Func<int, T> generator, Func<int> count)
        {
            return new Util.AnonymousReadOnlyList<T>
            {
                Count = count,
                GetItem = generator,
            };
        }

        /// <summary>
        /// Returns a read-only view that generates its elements when they are observed, passing the element's index to the generator delegate.
        /// </summary>
        /// <typeparam name="T">The type of elements observed by the view.</typeparam>
        /// <param name="generator">The delegate that is used to generate the elements.</param>
        /// <param name="count">The number of elements observed by the view. If <paramref name="count"/> is 0, an empty view is returned.</param>
        /// <returns>A read-only view that generates its elements on demand.</returns>
        public static IView<T> Generate<T>(Func<int, T> generator, int count)
        {
            return Generate<T>(generator, () => count);
        }

        /// <summary>
        /// Returns a read-only view that generates numbers in a sequence.
        /// </summary>
        /// <param name="start">The value at which the view begins (inclusive).</param>
        /// <param name="count">The number of elements in the view. This must be greater than or equal to <c>0</c>.</param>
        /// <param name="step">The stride of the view. Defaults to <c>1</c>.</param>
        /// <returns>A read-only view that generates numbers in a sequence.</returns>
        public static IView<int> Range(int start, int count, int step = 1)
        {
            return Generate<int>(i => start + i * step, () => count);
        }

        /// <summary>
        /// Returns an empty, read-only view. If you want a modifiable empty view, use <c>new List&lt;T&gt;.View()</c>.
        /// </summary>
        /// <typeparam name="T">The type of elements observed by the view.</typeparam>
        /// <returns>An empty, read-only view.</returns>
        public static IView<T> Empty<T>()
        {
            return new Util.AnonymousReadOnlyList<T>
            {
                Count = () => 0,
            };
        }

        /// <summary>
        /// Converts a single value into a read-only view which observes that value the specified number of times.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="source">The value.</param>
        /// <param name="count">The number of times <paramref name="source"/> is repeated. If <paramref name="count"/> is 0, an empty view is returned.</param>
        /// <returns>A read-only view observing <paramref name="count"/> elements, all equal to <paramref name="source"/>.</returns>
        public static IView<T> Repeat<T>(T source, int count)
        {
            return Generate<T>(_ => source, () => count);
        }

        /// <summary>
        /// Converts a single value into a read-only view which observes that value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="source">The value.</param>
        /// <returns>A read-only view observing a single element, <paramref name="source"/>.</returns>
        public static IView<T> Return<T>(T source)
        {
            return Generate<T>(_ => source, () => 1);
        }
    }
}
