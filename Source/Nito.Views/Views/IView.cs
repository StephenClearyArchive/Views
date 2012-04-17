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
    /// A view of data that provides fast random-access to elements. Views do not necessarily store data; they may be "virtual" in the sense of creating the data elements on request. Any type implementing this interface must also implement <see cref="INotifyCollectionChanged"/>, <see cref="ICanNotifyCollectionChanged"/>, and <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of item observed by the view.</typeparam>
    [ContractClass(typeof(ViewContracts<>))]
    public interface IView<out T>
    {
        int Count { get; }

        T this[int index] { get; }
    }

    public interface ICanNotifyCollectionChanged
    {
        bool CanNotifyCollectionChanged { get; }
    }
}
