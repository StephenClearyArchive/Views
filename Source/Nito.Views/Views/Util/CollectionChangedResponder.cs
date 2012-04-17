using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace Views.Util
{
    /// <summary>
    /// The callbacks for <see cref="CollectionChangedListener{T}"/>
    /// </summary>
    /// <typeparam name="T">The type of element contained in the collection.</typeparam>
    public interface ICollectionChangedResponder<T>
    {
        /// <summary>
        /// The collection added an item.
        /// </summary>
        /// <param name="collection">The collection that changed.</param>
        /// <param name="index">The index of the new item.</param>
        /// <param name="item">The item that was added.</param>
        void Added(INotifyCollectionChanged collection, int index, T item);

        /// <summary>
        /// The collection removed an item.
        /// </summary>
        /// <param name="collection">The collection that changed.</param>
        /// <param name="index">The index of the removed item.</param>
        /// <param name="item">The item that was removed.</param>
        void Removed(INotifyCollectionChanged collection, int index, T item);

        /// <summary>
        /// The collection replaced an item.
        /// </summary>
        /// <param name="collection">The collection that changed.</param>
        /// <param name="index">The index of the item that changed.</param>
        /// <param name="oldItem">The old item.</param>
        /// <param name="newItem">The new item.</param>
        void Replaced(INotifyCollectionChanged collection, int index, T oldItem, T newItem);

        /// <summary>
        /// The collection has undergone significant changes.
        /// </summary>
        /// <param name="collection">The collection that changed.</param>
        void Reset(INotifyCollectionChanged collection);
    }
}
