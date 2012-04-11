using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace Views.Util
{
    /// <summary>
    /// Helper classes and methods for implementing lists.
    /// </summary>
    public static class ListHelper
    {
        /// <summary>
        /// Returns a value indicating whether the specified collection itself may be updated, or <c>null</c> if it's not possible to know.
        /// </summary>
        /// <typeparam name="T">The type of elements contained in the source collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <returns>A value indicating whether the specified collection itself may be updated, or <c>null</c> if it's not possible to know.</returns>
        public static bool? CanUpdateCollection<T>(ICollection<T> source)
        {
            Contract.Requires(source != null);
            var list = source as System.Collections.IList;
            if (list != null)
                return !list.IsFixedSize;
            if (!source.IsReadOnly)
                return true;
            return null;
        }

        /// <summary>
        /// Returns a value indicating whether the elements within the specified collection may be updated, or <c>null</c> if it's not possible to know.
        /// </summary>
        /// <typeparam name="T">The type of elements contained in the source collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <returns>A value indicating whether the elements within the specified collection may be updated, or <c>null</c> if it's not possible to know.</returns>
        public static bool? CanUpdateElementValues<T>(ICollection<T> source)
        {
            Contract.Requires(source != null);
            var list = source as System.Collections.IList;
            if (list != null)
                return !list.IsReadOnly;
            if (!source.IsReadOnly)
                return true;
            return null;
        }

        /// <summary>
        /// Returns a value indicating whether all of the specified collections themselves may be updated, or <c>null</c> if it's not possible to know.
        /// </summary>
        /// <typeparam name="T">The type of elements contained in the source collections.</typeparam>
        /// <param name="sources">The source collections.</param>
        /// <returns>A value indicating whether all of the specified collections themselves may be updated, or <c>null</c> if it's not possible to know.</returns>
        public static bool? CanUpdateCollection<T>(IEnumerable<ICollection<T>> sources)
        {
            Contract.Requires(sources != null);
            Contract.Requires(Contract.ForAll(sources, x => x != null));
            var results = sources.Select(CanUpdateCollection);
            if (results.Any(x => x == false))
                return false;
            if (results.Any() && results.All(x => x == true))
                return true;
            return null;
        }

        /// <summary>
        /// Returns a value indicating whether the elements within all of the specified collections may be updated, or <c>null</c> if it's not possible to know.
        /// </summary>
        /// <typeparam name="T">The type of elements contained in the source collections.</typeparam>
        /// <param name="sources">The source collections.</param>
        /// <returns>A value indicating whether the elements within all of the specified collections may be updated, or <c>null</c> if it's not possible to know.</returns>
        public static bool? CanUpdateElementValues<T>(IEnumerable<ICollection<T>> sources)
        {
            Contract.Requires(sources != null);
            Contract.Requires(Contract.ForAll(sources, x => x != null));
            var results = sources.Select(CanUpdateElementValues);
            if (results.Any(x => x == false))
                return false;
            if (results.Any() && results.All(x => x == true))
                return true;
            return null;
        }

        /// <summary>
        /// Returns a value indicating whether all of the specified collections themselves may be updated, or <c>null</c> if it's not possible to know.
        /// </summary>
        /// <typeparam name="T">The type of elements contained in the source collections.</typeparam>
        /// <param name="sources">The source collections.</param>
        /// <returns>A value indicating whether all of the specified collections themselves may be updated, or <c>null</c> if it's not possible to know.</returns>
        public static bool? CanUpdateCollection<T>(params ICollection<T>[] sources)
        {
            Contract.Requires(sources != null);
            Contract.Requires(Contract.ForAll(sources, x => x != null));
            return CanUpdateCollection((IEnumerable<ICollection<T>>)sources);
        }

        /// <summary>
        /// Returns a value indicating whether the elements within all of the specified collections may be updated, or <c>null</c> if it's not possible to know.
        /// </summary>
        /// <typeparam name="T">The type of elements contained in the source collections.</typeparam>
        /// <param name="sources">The source collections.</param>
        /// <returns>A value indicating whether the elements within all of the specified collections may be updated, or <c>null</c> if it's not possible to know.</returns>
        public static bool? CanUpdateElementValues<T>(params ICollection<T>[] sources)
        {
            Contract.Requires(sources != null);
            Contract.Requires(Contract.ForAll(sources, x => x != null));
            return CanUpdateElementValues((IEnumerable<ICollection<T>>)sources);
        }
    }
}
