using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Views.Util
{
    /// <summary>
    /// A disposable that disposes a collection of disposables when it is disposed.
    /// </summary>
    public sealed class MultiDispose : IDisposable
    {
        /// <summary>
        /// The collection of disposable objects. Elements in this collection may be <c>null</c>.
        /// </summary>
        private IEnumerable<IDisposable> disposables;
        
        /// <summary>
        /// Creates a new instance of the <see cref="MultiDispose"/> class, which will dispose the specified collection of disposables when it is disposed.
        /// </summary>
        /// <param name="disposables">The collection of disposable objects. Elements in this collection may be <c>null</c>.</param>
        public MultiDispose(IEnumerable<IDisposable> disposables)
        {
            this.disposables = disposables;
        }

        /// <summary>
        /// Disposes all disposables in the collection, if they have not already been disposed.
        /// </summary>
        void IDisposable.Dispose()
        {
            if (this.disposables == null)
                return;

            foreach (var disposable in this.disposables)
            {
                if (disposable != null)
                    disposable.Dispose();
            }

            this.disposables = null;
        }
    }
}
