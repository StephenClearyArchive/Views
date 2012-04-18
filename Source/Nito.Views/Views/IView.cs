using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using System.Collections.Specialized;

namespace Views
{
    [ContractClassFor(typeof(IView<>))]
    internal abstract class ViewContracts<T> : IView<T>
    {
        public int Count
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);
                return 0;
            }
        }

        public T this[int index]
        {
            get
            {
                Contract.Requires(index >= 0 && index < Count);
                return default(T);
            }
        }
    }

    /// <summary>
    /// A view of data that provides fast random-access to elements. Views do not necessarily store data; they may be "virtual" in the sense of creating the data elements on request. Any type implementing this interface must also implement <see cref="INotifyCollectionChanged"/>, <see cref="Views.Util.ICanNotifyCollectionChanged"/>, and <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of item observed by the view.</typeparam>
    [ContractClass(typeof(ViewContracts<>))]
    public interface IView<out T> : Views.Linq.IView
    {
        /// <summary>
        /// Gets the number of elements observed by this view.
        /// </summary>
        /// <returns>The number of elements observed by this view.</returns>
        int Count { get; }

        /// <summary>
        /// Gets the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item to get.</param>
        T this[int index] { get; }
    }

    /// <summary>
    /// A generator of permutations of another view.
    /// </summary>
    /// <typeparam name="T">The type of item observed by the view.</typeparam>
    public interface IViewPermutationGenerator<out T>
    {
        /// <summary>
        /// Gets the current permuation of the view. This value is only valid until <see cref="NextPermutation"/> is invoked.
        /// </summary>
        IView<T> Current { get; }

        /// <summary>
        /// Rearranges the elements in this view to advance to the next permutation. If this method returns <c>false</c>, then there are no more permutations, and this view is reset to the first permutation.
        /// </summary>
        /// <returns><c>true</c> if this view advanced to the next permutation; <c>false</c> if this view reset to the first permutation.</returns>
        bool NextPermutation();
    }
}
