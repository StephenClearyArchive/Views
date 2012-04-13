using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace Views.Util
{
    /// <summary>
    /// An indirect list, which provides a layer of indirection for the index values of a source list.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    public sealed class IndirectList<T> : IndirectListBase<T>, ICollectionChangedResponder<int>
    {
        /// <summary>
        /// The listener for the list of redirected index values.
        /// </summary>
        private readonly CollectionChangedListener<int> indicesListener;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndirectList&lt;T&gt;"/> class for the given source list, using the given redirected index values.
        /// </summary>
        /// <param name="source">The source list.</param>
        /// <param name="indices">The redirected index values. If this is <c>null</c>, then a new list of indices is created matching the current source indices.</param>
        public IndirectList(IList<T> source, IList<int> indices = null)
            : base(source, indices ?? DefaultIndices(source))
        {
            Contract.Requires(source != null);
            this.indicesListener = CollectionChangedListener<int>.Create(this.indices, this);
        }

        /// <summary>
        /// Gets the source list.
        /// </summary>
        public IList<T> SourceList
        {
            get
            {
                Contract.Ensures(Contract.Result<IList<T>>() != null);
                return this.source;
            }
        }

        /// <summary>
        /// Gets the redirected index values.
        /// </summary>
        public IList<int> ListIndices
        {
            get
            {
                Contract.Ensures(Contract.Result<IList<int>>() != null);
                return this.indices;
            }
        }

        void ICollectionChangedResponder<int>.Added(int index, int item)
        {
            this.CreateNotifier().Added(index, this.source[item]);
        }

        void ICollectionChangedResponder<int>.Removed(int index, int item)
        {
            if (item < this.source.Count)
                this.CreateNotifier().Removed(index, this.source[item]);
            else
                this.CreateNotifier().Reset();
        }

        void ICollectionChangedResponder<int>.Replaced(int index, int oldItem, int newItem)
        {
            if (oldItem < this.source.Count)
                this.CreateNotifier().Replaced(index, this.source[oldItem], this.source[newItem]);
            else
                this.CreateNotifier().Reset();
        }

        void ICollectionChangedResponder<int>.Reset()
        {
            this.CreateNotifier().Reset();
        }

        /// <summary>
        /// A notification that there is at least one <see cref="ListBase{T}.CollectionChanged"/> or <see cref="ListBase{T}.PropertyChanged"/> subscription active. This implementation activates the source listener.
        /// </summary>
        protected override void SubscriptionsActive()
        {
            this.indicesListener.Activate();
            base.SubscriptionsActive();
        }

        /// <summary>
        /// A notification that there are no <see cref="ListBase{T}.CollectionChanged"/> nor <see cref="ListBase{T}.PropertyChanged"/> subscriptions active. This implementation deactivates the source listener.
        /// </summary>
        protected override void SubscriptionsInactive()
        {
            this.indicesListener.Deactivate();
            base.SubscriptionsInactive();
        }

        /// <summary>
        /// Gets an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get. This index is guaranteed to be valid.</param>
        /// <returns>The element at the specified index.</returns>
        protected override T DoGetItem(int index)
        {
            using (this.indicesListener.Pause())
            {
                return base.DoGetItem(index);
            }
        }

        /// <summary>
        /// Sets an element at the specified index. This implementation always throws <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get. This index is guaranteed to be valid.</param>
        /// <param name="item">The element to store in the list.</param>
        protected override void DoSetItem(int index, T item)
        {
            using (this.indicesListener.Pause())
            {
                base.DoSetItem(index, item);
            }
        }
    }
}
