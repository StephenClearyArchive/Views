using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Views.Util
{
    /// <summary>
    /// Indexes into a source list using a step size.
    /// </summary>
    /// <typeparam name="T">The type of object contained in the list.</typeparam>
    public sealed class StepList<T> : ReadOnlyListBase<T>, CollectionChangedListener<T>.IResponder
    {
        /// <summary>
        /// The source list.
        /// </summary>
        private readonly IList<T> source;

        /// <summary>
        /// The listener for the source list.
        /// </summary>
        private readonly CollectionChangedListener<T> listener;

        /// <summary>
        /// The step size to use when traversing the source list.
        /// </summary>
        private readonly int step;

        /// <summary>
        /// Initializes a new instance of the <see cref="StepList&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="source">The source list.</param>
        /// <param name="step">The step size to use when traversing the source list.</param>
        public StepList(IList<T> source, int step)
        {
            if (step <= 0)
            {
                throw new ArgumentOutOfRangeException("step", "The step parameter must be greater than 0");
            }

            this.source = source;
            this.listener = CollectionChangedListener<T>.Create(source, this);
            this.step = step;
        }

        void CollectionChangedListener<T>.IResponder.Added(int index, T item)
        {
            this.CreateNotifier().Reset();
        }

        void CollectionChangedListener<T>.IResponder.Removed(int index, T item)
        {
            this.CreateNotifier().Reset();
        }

        void CollectionChangedListener<T>.IResponder.Replaced(int index, T oldItem, T newItem)
        {
            if (index % this.step == 0)
                this.CreateNotifier().Replaced(index / this.step, oldItem, newItem);
        }

        void CollectionChangedListener<T>.IResponder.Reset()
        {
            this.CreateNotifier().Reset();
        }

        /// <summary>
        /// A notification that there is at least one <see cref="CollectionChanged"/> or <see cref="PropertyChanged"/> subscription active. This implementation activates the source listener.
        /// </summary>
        protected override void SubscriptionsActive()
        {
            this.listener.Activate();
        }

        /// <summary>
        /// A notification that there are no <see cref="CollectionChanged"/> nor <see cref="PropertyChanged"/> subscriptions active. This implementation deactivates the source listener.
        /// </summary>
        protected override void SubscriptionsInactive()
        {
            this.listener.Deactivate();
        }

        /// <summary>
        /// Returns a value indicating whether the elements within this collection may be updated, e.g., the index setter.
        /// </summary>
        /// <returns>A value indicating whether the elements within this collection may be updated.</returns>
        protected override bool CanUpdateElementValues()
        {
            return true;
        }

        /// <summary>
        /// Gets the number of elements contained in this list.
        /// </summary>
        /// <returns>The number of elements contained in this list.</returns>
        protected override int DoCount()
        {
            if (this.source.Count == 0)
            {
                return 0;
            }

            return ((this.source.Count - 1) / this.step) + 1;
        }

        /// <summary>
        /// Gets an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get. This index is guaranteed to be valid.</param>
        /// <returns>The element at the specified index.</returns>
        protected override T DoGetItem(int index)
        {
            return this.source[index * this.step];
        }

        /// <summary>
        /// Sets an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get. This index is guaranteed to be valid.</param>
        /// <param name="item">The element to store in the list.</param>
        protected override void DoSetItem(int index, T item)
        {
            using (this.listener.Pause())
            {
                this.source[index * this.step] = item;
            }
        }
    }
}
