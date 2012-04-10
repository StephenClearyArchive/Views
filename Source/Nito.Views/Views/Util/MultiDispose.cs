using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace Views.Util
{
    /// <summary>
    /// A disposable that disposes a collection of disposables when it is disposed. It is normal for instances of this class to be <c>null</c>.
    /// </summary>
    public sealed class MultiDispose : IDisposable
    {
        /// <summary>
        /// The collection of disposable objects. Elements in this collection may not be <c>null</c>.
        /// </summary>
        private List<IDisposable> disposables;
        
        /// <summary>
        /// Creates a new instance of the <see cref="MultiDispose"/> class, which will dispose the specified collection of disposables when it is disposed.
        /// </summary>
        /// <param name="disposables">The collection of disposable objects. Elements in this collection may not be <c>null</c>.</param>
        private MultiDispose(List<IDisposable> disposables)
        {
            Contract.Requires(disposables != null);
            this.disposables = disposables;
        }

        /// <summary>
        /// Gets the collection of disposable objects. Elements in this collection may not be <c>null</c>.
        /// </summary>
        public List<IDisposable> Disposables
        {
            get { return this.disposables; }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.disposables != null);
        }

        /// <summary>
        /// Disposes all disposables in the collection, if they have not already been disposed.
        /// </summary>
        void IDisposable.Dispose()
        {
            if (this.disposables == null)
                return;

            foreach (var disposable in this.disposables)
                disposable.Dispose();

            this.disposables = null;
        }

        /// <summary>
        /// Creates a new <see cref="MultiDispose"/> to dispose the specified disposables. Returns <c>null</c> if <paramref name="disposables"/> does not contain any disposables.
        /// </summary>
        /// <param name="disposables">The disposables to wrap.</param>
        /// <returns>A new <see cref="MultiDispose"/> or <c>null</c>.</returns>
        public static MultiDispose Create(IEnumerable<IDisposable> disposables)
        {
            Contract.Requires(disposables != null);
            var filteredDisposables = disposables.Where(x => x != null);
            if (!filteredDisposables.Any())
                return null;
            return new MultiDispose(filteredDisposables.ToList());
        }

        /// <summary>
        /// Creates a new <see cref="MultiDispose"/> to dispose the specified disposables. Returns <c>null</c> if <paramref name="disposables"/> does not contain any disposables.
        /// </summary>
        /// <param name="disposables">The disposables to wrap.</param>
        /// <returns>A new <see cref="MultiDispose"/> or <c>null</c>.</returns>
        public static MultiDispose Create(params IDisposable[] disposables)
        {
            Contract.Requires(disposables != null);
            return Create((IEnumerable<IDisposable>)disposables);
        }
    }

    /// <summary>
    /// Provides extension methods for the <see cref="MultiDispose"/> class. These methods work for <c>null</c> instances.
    /// </summary>
    public static class MultiDisposeExtensions
    {
        /// <summary>
        /// Appends a series of disposables to the <see cref="MultiDispose"/> instance (which may be <c>null</c>). Returns <c>null</c> if <paramref name="disposable"/> is <c>null</c> and <paramref name="disposables"/> does not contain any disposables.
        /// </summary>
        /// <param name="disposable">The existing <see cref="MultiDispose"/> instance, which may be <c>null</c>.</param>
        /// <param name="disposables">The additional disposables to wrap.</param>
        /// <returns>A <see cref="MultiDispose"/> that will dispose all specified disposables, or <c>null</c>.</returns>
        public static MultiDispose Add(this MultiDispose disposable, IEnumerable<IDisposable> disposables)
        {
            Contract.Requires(disposables != null);
            if (disposable == null)
                return MultiDispose.Create(disposables);
            disposable.Disposables.AddRange(disposables.Where(x => x != null));
            return disposable;
        }

        /// <summary>
        /// Appends a series of disposables to the <see cref="MultiDispose"/> instance (which may be <c>null</c>). Returns <c>null</c> if <paramref name="disposable"/> is <c>null</c> and <paramref name="disposables"/> does not contain any disposables.
        /// </summary>
        /// <param name="disposable">The existing <see cref="MultiDispose"/> instance, which may be <c>null</c>.</param>
        /// <param name="disposables">The additional disposables to wrap.</param>
        /// <returns>A <see cref="MultiDispose"/> that will dispose all specified disposables, or <c>null</c>.</returns>
        public static MultiDispose Add(this MultiDispose disposable, params IDisposable[] disposables)
        {
            Contract.Requires(disposables != null);
            return Add(disposable, (IEnumerable<IDisposable>)disposables);
        }
    }
}
