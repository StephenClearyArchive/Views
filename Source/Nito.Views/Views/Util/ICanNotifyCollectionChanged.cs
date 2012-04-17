using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace Views.Util
{
    /// <summary>
    /// Adds a property to <see cref="INotifyCollectionChanged"/> that indicates whether a given instance will ever raise <see cref="INotifyCollectionChanged.CollectionChanged"/>.
    /// </summary>
    public interface ICanNotifyCollectionChanged : INotifyCollectionChanged
    {
        /// <summary>
        /// Returns a value indicating whether an instance may ever raise <see cref="INotifyCollectionChanged.CollectionChanged"/>.
        /// </summary>
        bool CanNotifyCollectionChanged { get; }
    }
}
