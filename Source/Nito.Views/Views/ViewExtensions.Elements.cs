﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace Views
{
    /// <summary>
    /// Provides extension methods for all views.
    /// </summary>
    public static partial class ViewExtensions
    {
        /// <summary>
        /// Returns the element at a specified index.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The view.</param>
        /// <param name="index">The index of the element to return.</param>
        /// <returns>The element at the specified index.</returns>
        public static T ElementAt<T>(this IView<T> view, int index)
        {
            Contract.Requires(view != null);
            Contract.Requires(view is IList<T>);
            Contract.Requires(index >= 0);
            Contract.Requires(index < (view as IList<T>).Count);
            return (view as IList<T>)[index];
        }

        /// <summary>
        /// Returns the element at a specified index, or a default value if the index is invalid.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The view.</param>
        /// <param name="index">The index of the element to return.</param>
        /// <param name="defaultValue">The default value to return if the index is invalid. Defaults to <c>default(T)</c>.</param>
        /// <returns>The element at the specified index.</returns>
        public static T ElementAtOrDefault<T>(this IView<T> view, int index, T defaultValue = default(T))
        {
            Contract.Requires(view != null);
            Contract.Requires(view is IList<T>);
            var list = view as IList<T>;
            if (index < 0 || index >= list.Count)
                return defaultValue;
            return list[index];
        }

        /// <summary>
        /// Returns the first element in a view. Throws <see cref="InvalidOperationException"/> if the view is empty.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The view.</param>
        /// <returns>The first element in a view.</returns>
        public static T First<T>(this IView<T> view)
        {
            Contract.Requires(view != null);
            Contract.Requires(view is IList<T>);
            var list = view as IList<T>;
            if (list.Count == 0)
                throw new InvalidOperationException("View contains no elements.");
            return list[0];
        }

        /// <summary>
        /// Returns the first element in a view that matches a condition. Throws <see cref="InvalidOperationException"/> if no elements in the view match the condition.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The view.</param>
        /// <param name="predicate">The condition used to evaluate elements.</param>
        /// <returns>The first element in a view that matches a condition.</returns>
        public static T First<T>(this IView<T> view, Func<T, bool> predicate)
        {
            Contract.Requires(view != null);
            Contract.Requires(view is IList<T>);
            Contract.Requires(predicate != null);
            var index = view.FirstIndex(predicate);
            if (index == -1)
                throw new InvalidOperationException("View contains no matching elements.");
            return (view as IList<T>)[index];
        }

        /// <summary>
        /// Returns the first element in a view, or <paramref name="defaultValue"/> if the view is empty.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The view.</param>
        /// <param name="defaultValue">The default value to return if the view is empty. Defaults to <c>default(T)</c>.</param>
        /// <returns>The first element in a view, or <paramref name="defaultValue"/> if the view is empty..</returns>
        public static T FirstOrDefault<T>(this IView<T> view, T defaultValue = default(T))
        {
            Contract.Requires(view != null);
            Contract.Requires(view is IList<T>);
            var list = view as IList<T>;
            if (list.Count == 0)
                return defaultValue;
            return list[0];
        }

        /// <summary>
        /// Returns the first element in a view that matches a condition, or <paramref name="defaultValue"/> if no elements in the view match the condition.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The view.</param>
        /// <param name="predicate">The condition used to evaluate elements.</param>
        /// <param name="defaultValue">The default value to return if no elements in the view match the condition. Defaults to <c>default(T)</c>.</param>
        /// <returns>The first element in a view that matches a condition, or <paramref name="defaultValue"/> if no elements in the view match the condition.</returns>
        public static T FirstOrDefault<T>(this IView<T> view, Func<T, bool> predicate, T defaultValue = default(T))
        {
            Contract.Requires(view != null);
            Contract.Requires(view is IList<T>);
            Contract.Requires(predicate != null);
            var index = view.FirstIndex(predicate);
            if (index == -1)
                return defaultValue;
            return (view as IList<T>)[index];
        }

        /// <summary>
        /// Returns the last element in a view. Throws <see cref="InvalidOperationException"/> if the view is empty.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The view.</param>
        /// <returns>The first element in a view.</returns>
        public static T Last<T>(this IView<T> view)
        {
            Contract.Requires(view != null);
            Contract.Requires(view is IList<T>);
            var list = view as IList<T>;
            var count = list.Count;
            if (count == 0)
                throw new InvalidOperationException("View contains no elements.");
            return list[count - 1];
        }

        /// <summary>
        /// Returns the last element in a view that matches a condition. Throws <see cref="InvalidOperationException"/> if no elements in the view match the condition.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The view.</param>
        /// <param name="predicate">The condition used to evaluate elements.</param>
        /// <returns>The last element in a view that matches a condition.</returns>
        public static T Last<T>(this IView<T> view, Func<T, bool> predicate)
        {
            Contract.Requires(view != null);
            Contract.Requires(view is IList<T>);
            Contract.Requires(predicate != null);
            var index = view.LastIndex(predicate);
            if (index == -1)
                throw new InvalidOperationException("View contains no matching elements.");
            return (view as IList<T>)[index];
        }

        /// <summary>
        /// Returns the last element in a view, or <paramref name="defaultValue"/> if the view is empty.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The view.</param>
        /// <param name="defaultValue">The default value to return if the view is empty. Defaults to <c>default(T)</c>.</param>
        /// <returns>The last element in a view, or <paramref name="defaultValue"/> if the view is empty..</returns>
        public static T LastOrDefault<T>(this IView<T> view, T defaultValue = default(T))
        {
            Contract.Requires(view != null);
            Contract.Requires(view is IList<T>);
            var list = view as IList<T>;
            var count = list.Count;
            if (count == 0)
                return defaultValue;
            return list[count - 1];
        }

        /// <summary>
        /// Returns the last element in a view that matches a condition, or <paramref name="defaultValue"/> if no elements in the view match the condition.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The view.</param>
        /// <param name="predicate">The condition used to evaluate elements.</param>
        /// <param name="defaultValue">The default value to return if no elements in the view match the condition. Defaults to <c>default(T)</c>.</param>
        /// <returns>The last element in a view that matches a condition, or <paramref name="defaultValue"/> if no elements in the view match the condition.</returns>
        public static T LastOrDefault<T>(this IView<T> view, Func<T, bool> predicate, T defaultValue = default(T))
        {
            Contract.Requires(view != null);
            Contract.Requires(view is IList<T>);
            Contract.Requires(predicate != null);
            var index = view.LastIndex(predicate);
            if (index == -1)
                return defaultValue;
            return (view as IList<T>)[index];
        }

        /// <summary>
        /// Returns the only element in a view. Throws <see cref="InvalidOperationException"/> if the view does not contain exactly one element.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The view.</param>
        /// <returns>The only element in a view.</returns>
        public static T Single<T>(this IView<T> view)
        {
            Contract.Requires(view != null);
            Contract.Requires(view is IList<T>);
            var list = view as IList<T>;
            var count = list.Count;
            if (count != 1)
                throw new InvalidOperationException("View does not contain exactly one element.");
            return list[0];
        }

        /// <summary>
        /// Returns the only element in a view that matches a condition. Throws <see cref="InvalidOperationException"/> if there is not exactly one element in the view that matches the condition.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The view.</param>
        /// <param name="predicate">The condition used to evaluate elements.</param>
        /// <returns>The only element in a view that matches a condition.</returns>
        public static T Single<T>(this IView<T> view, Func<T, bool> predicate)
        {
            Contract.Requires(view != null);
            Contract.Requires(view is IList<T>);
            Contract.Requires(predicate != null);
            var index = view.SingleIndex(predicate);
            if (index == -1)
                throw new InvalidOperationException("View contains no matching elements.");
            return (view as IList<T>)[index];
        }

        /// <summary>
        /// Returns the only element in a view that matches a condition, or <paramref name="defaultValue"/> if no elements in the view match the condition. Throws <see cref="InvalidOperationException"/> if multiple elements match the condition.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The view.</param>
        /// <param name="predicate">The condition used to evaluate elements.</param>
        /// <param name="defaultValue">The default value to return if no elements in the view match the condition. Defaults to <c>default(T)</c>.</param>
        /// <returns>The last element in a view that matches a condition, or <paramref name="defaultValue"/> if no elements in the view match the condition.</returns>
        public static T SingleOrDefault<T>(this IView<T> view, Func<T, bool> predicate, T defaultValue = default(T))
        {
            Contract.Requires(view != null);
            Contract.Requires(view is IList<T>);
            Contract.Requires(predicate != null);
            var index = view.SingleIndex(predicate);
            if (index == -1)
                return defaultValue;
            return (view as IList<T>)[index];
        }

        /// <summary>
        /// Returns the largest element in a view.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The view.</param>
        /// <param name="comparer">The comparison object used to evaluate elements. Defaults to <c>null</c>. If this parameter is <c>null</c>, then this method uses the default comparison object.</param>
        /// <returns>The largest element in the view.</returns>
        public static T Max<T>(this IView<T> view, IComparer<T> comparer = null)
        {
            Contract.Requires(view != null);
            Contract.Requires(view is IList<T>);
            var index = view.MaxIndex(comparer);
            if (index == -1)
                return default(T);
            return (view as IList<T>)[index];
        }

        /// <summary>
        /// Returns the smallest element in a view.
        /// </summary>
        /// <typeparam name="T">The type of element observed by the view.</typeparam>
        /// <param name="view">The view.</param>
        /// <param name="comparer">The comparison object used to evaluate elements. Defaults to <c>null</c>. If this parameter is <c>null</c>, then this method uses the default comparison object.</param>
        /// <returns>The smallest element in the view.</returns>
        public static T Min<T>(this IView<T> view, IComparer<T> comparer = null)
        {
            Contract.Requires(view != null);
            Contract.Requires(view is IList<T>);
            var index = view.MinIndex(comparer);
            if (index == -1)
                return default(T);
            return (view as IList<T>)[index];
        }
    }
}
