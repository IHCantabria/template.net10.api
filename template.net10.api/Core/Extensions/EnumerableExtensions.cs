using System.Diagnostics.CodeAnalysis;
using LanguageExt;

namespace template.net10.api.Core.Extensions;

/// <summary>
///     ADD DOCUMENTATION
/// </summary>
[SuppressMessage(
    "ReSharper",
    "UnusedType.Global",
    Justification = "General-purpose helper type; usage depends on consumer requirements.")]
[SuppressMessage(
    "ReSharper",
    "UnusedMember.Global",
    Justification = "General-purpose helper methods; not all members are used in every scenario.")]
internal static class EnumerableExtensions
{
    /// <summary>
    ///     ADD DOCUMENTATION
    /// </summary>
    private static TR FindMinValue<T, TR>(IEnumerator<T> enumerator, Func<T, TR> valuingFoo)
        where TR : IComparable
    {
        if (!enumerator.MoveNext()) throw new ArgumentException("Container is empty!");

        var minVal = valuingFoo(enumerator.Current);

        while (enumerator.MoveNext())
        {
            var currentVal = valuingFoo(enumerator.Current);
            if (currentVal.CompareTo(minVal) < 0) minVal = currentVal;
        }

        return minVal;
    }

    /// <summary>
    ///     ADD DOCUMENTATION
    /// </summary>
    private static T FindMinElement<T, TR>(IEnumerator<T> enumerator, Func<T, TR> valuingFoo, TR minValue)
        where TR : IComparable
    {
        enumerator.Reset(); // Reset the enumerator to start from the beginning

        while (enumerator.MoveNext())
        {
            var currentVal = valuingFoo(enumerator.Current);
            if (currentVal.CompareTo(minValue) == 0) return enumerator.Current;
        }

        throw new InvalidOperationException("Minimum element not found!");
    }

    /// <summary>
    ///     ADD DOCUMENTATION
    /// </summary>
    private static TR FindMaxValue<T, TR>(IEnumerator<T> enumerator, Func<T, TR> valuingFoo)
        where TR : IComparable
    {
        if (!enumerator.MoveNext()) throw new ArgumentException("Container is empty!");

        var maxVal = valuingFoo(enumerator.Current);

        while (enumerator.MoveNext())
        {
            var currentVal = valuingFoo(enumerator.Current);
            if (currentVal.CompareTo(maxVal) > 0) maxVal = currentVal;
        }

        return maxVal;
    }

    /// <summary>
    ///     ADD DOCUMENTATION
    /// </summary>
    private static T FindMaxElement<T, TR>(IEnumerator<T> enumerator, Func<T, TR> valuingFoo, TR maxValue)
        where TR : IComparable
    {
        enumerator.Reset(); // Reset the enumerator to start from the beginning

        while (enumerator.MoveNext())
        {
            var currentVal = valuingFoo(enumerator.Current);
            if (currentVal.CompareTo(maxValue) == 0) return enumerator.Current;
        }

        throw new ArgumentException("Enumerator is empty!");
    }

    /// <summary>
    ///     ADD DOCUMENTATION
    /// </summary>
    private static List<T> FindMaxElements<T, TR>(IEnumerator<T> enumerator,
        Func<T, TR> valuingFoo)
        where TR : IComparable
    {
        // Find the maximum values
        var maxValues = FindMaxValues(enumerator, valuingFoo);

        // Find the elements corresponding to the maximum value
        return FindMaxElements(enumerator, valuingFoo, maxValues[0]);
    }

    /// <summary>
    ///     ADD DOCUMENTATION
    /// </summary>
    private static List<T> FindMaxElements<T, TR>(IEnumerator<T> enumerator, Func<T, TR> valuingFoo, TR maxValue)
        where TR : IComparable
    {
        var maxElements = new List<T>();

        enumerator.Reset(); // Resetting the enumerator to start from the beginning

        while (enumerator.MoveNext())
        {
            var currentVal = valuingFoo(enumerator.Current);
            if (currentVal.CompareTo(maxValue) == 0) maxElements.Add(enumerator.Current);
        }

        return maxElements;
    }

    /// <summary>
    ///     ADD DOCUMENTATION
    /// </summary>
    private static List<TR> FindMaxValues<T, TR>(IEnumerator<T> enumerator, Func<T, TR> valuingFoo)
        where TR : IComparable
    {
        var maxValues = new List<TR> { valuingFoo(enumerator.Current) };

        while (enumerator.MoveNext())
        {
            var currentVal = valuingFoo(enumerator.Current);

            switch (currentVal.CompareTo(maxValues[0]))
            {
                case < 0:
                    continue;
                case 0:
                    maxValues.Add(currentVal);
                    break;
                default:
                    maxValues = [currentVal];
                    break;
            }
        }

        return maxValues;
    }

    extension<T>(IEnumerable<T> container)
    {
        /// <summary>
        ///     ADD DOCUMENTATION
        /// </summary>
        /// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created.</exception>
        /// <exception cref="ArgumentException">Container is empty!</exception>
        internal Try<IEnumerable<T>> MaxElements<TR>(Func<T, TR> valuingFoo)
            where TR : IComparable
        {
            return () =>
            {
                using var enumerator = container.GetEnumerator();

                return !enumerator.MoveNext()
                    ? throw new ArgumentException("Container is empty!")
                    : FindMaxElements(enumerator, valuingFoo);
            };
        }
    }

    extension<T>(IEnumerable<T> container)
    {
        /// <summary>
        ///     ADD DOCUMENTATION
        /// </summary>
        /// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created.</exception>
        /// <exception cref="NotSupportedException">The enumerator does not support being reset.</exception>
        internal Try<T> MaxElement<TR>(Func<T, TR> valuingFoo)
            where TR : IComparable
        {
            return () =>
            {
                using var enumerator = container.GetEnumerator();

                // Find the maximum value
                var maxValue = FindMaxValue(enumerator, valuingFoo);

                // Reset the enumerator and find the element corresponding to the maximum value
                enumerator.Reset();
                return FindMaxElement(enumerator, valuingFoo, maxValue);
            };
        }
    }

    extension<T>(IEnumerable<T> container)
    {
        /// <summary>
        ///     ADD DOCUMENTATION
        /// </summary>
        /// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created.</exception>
        /// <exception cref="NotSupportedException">The enumerator does not support being reset.</exception>
        internal Try<T> MinElement<TR>(Func<T, TR> valuingFoo)
            where TR : IComparable
        {
            return () =>
            {
                using var enumerator = container.GetEnumerator();

                // Find the minimum value
                var minValue = FindMinValue(enumerator, valuingFoo);

                // Reset the enumerator and find the element corresponding to the minimum value
                enumerator.Reset(); // Resetting the enumerator
                return FindMinElement(enumerator, valuingFoo, minValue);
            };
        }
    }
}