using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace Views.Util
{
    /// <summary>
    /// Provides common implementations of some view methods.
    /// </summary>
    /// <typeparam name="T">The type of element observed by the view.</typeparam>
    public abstract class ConstantViewBase<T> : ViewBase<T>, INotifyCollectionChanged, ICanNotifyCollectionChanged
    {
        public bool CanNotifyCollectionChanged
        {
            get { return false; }
        }

        /// <summary>
        /// Notifies listeners of changes in the view.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { }
            remove { }
        }
    }
}
