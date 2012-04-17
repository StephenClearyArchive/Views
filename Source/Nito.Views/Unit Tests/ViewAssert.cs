using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Views;
using System.Collections.Specialized;

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
        AreEquivalent<T>(expected, actual as IEnumerable<T>);
    }

    public static void AreEquivalent<T>(IView<T> expected, IView<T> actual)
    {
        AreEquivalent<T>(expected as IEnumerable<T>, actual as IEnumerable<T>);
    }

    public static void BasicFunctionality<T>(IView<T> view)
    {
        // Views implement IEnumerable<T>, INotifyCollectionChanged, and ICanNotifyCollectionChanged.
        var enumerable = view as IEnumerable<T>;
        Assert.IsNotNull(enumerable);
        Assert.IsNotNull(view as INotifyCollectionChanged);
        Assert.IsNotNull(view as ICanNotifyCollectionChanged);

        // Invalid Item indexes are rejected.
        ThrowsException(() => { var test = view[-1]; });
        ThrowsException(() => { var test = view[view.Count]; });
    }
}
