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
}
