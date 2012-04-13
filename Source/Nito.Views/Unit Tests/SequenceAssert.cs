using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Views;

public static class SequenceAssert
{
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
}
