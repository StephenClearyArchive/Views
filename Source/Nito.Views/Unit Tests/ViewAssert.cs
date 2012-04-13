using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Views;

public static class ViewAssert
{
    private static void ThrowsException(Action action)
    {
        try
        {
            action();
        }
        catch
        {
            return;
        }

        Assert.Fail("Test did not throw an exception.");
    }

    public static void AreEquivalent<T>(IEnumerable<T> expected, IEnumerable<T> actual)
    {
        CollectionAssert.AreEquivalent(expected.ToList(), actual.ToList());
    }

    public static void AreEquivalent<T>(IEnumerable<T> expected, IView<T> actual)
    {
        AreEquivalent<T>(expected, actual as IList<T>);
    }

    public static void AreEquivalent<T>(IView<T> expected, IView<T> actual)
    {
        AreEquivalent<T>(expected as IList<T>, actual as IList<T>);
    }

    public static void ElementsAreReadOnly<T>(IView<T> view)
    {
        var list = view as IList<T>;
        var objectList = view as System.Collections.IList;
        Assert.IsTrue(list.IsReadOnly);
        Assert.IsTrue(objectList.IsReadOnly);
        Assert.IsTrue(list.Count > 0);
        ThrowsException(() => { list[0] = default(T); });
    }

    public static void CollectionIsReadOnly<T>(IView<T> view)
    {
        var list = view as IList<T>;
        var objectList = view as System.Collections.IList;
        Assert.IsTrue(list.IsReadOnly);
        Assert.IsTrue(objectList.IsFixedSize);
        ThrowsException(() => { list.Insert(0, default(T)); });
        ThrowsException(() => { list.RemoveAt(0); });
        ThrowsException(() => { list.Clear(); });
    }

    public static void BasicFunctionality<T>(IView<T> view)
    {
        // Views implement both IList<T> and IList.
        var list = view as IList<T>;
        var objectList = view as System.Collections.IList;
        Assert.IsNotNull(list);
        Assert.IsNotNull(objectList);

        // Both have the same count.
        Assert.AreEqual(list.Count, objectList.Count);

        // IList<T>.IsReadOnly = IList.IsReadOnly || IList.IsFixedSize
        Assert.AreEqual(list.IsReadOnly, objectList.IsReadOnly || objectList.IsFixedSize);

        // IsSynchronized is false, but SyncRoot is an independent object.
        Assert.IsFalse(objectList.IsSynchronized);
        Assert.IsNotNull(objectList.SyncRoot);
        Assert.AreNotSame(view, objectList.SyncRoot);

        // Invalid Item indexes are rejected.
        ThrowsException(() => { var test = list[-1]; });
        ThrowsException(() => { var test = list[list.Count]; });
        ThrowsException(() => { var test = objectList[-1]; });
        ThrowsException(() => { var test = objectList[list.Count]; });

        // Both have the same elements.
        AreEquivalent(list, objectList.Cast<T>());

        // For each list, accessing via the indexer is the same as the enumerable implementation.
        AreEquivalent(Enumerable.Range(0, list.Count).Select(i => list[i]), list);
        AreEquivalent(Enumerable.Range(0, objectList.Count).Select(i => objectList[i]).Cast<T>(), list);

        // TODO: Contains, IndexOf, CopyTo.
    }
}
