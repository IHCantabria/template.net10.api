using System.Diagnostics.CodeAnalysis;
using LanguageExt;

namespace template.net10.api.Core.Extensions;

/// <summary>
///     Provides extension methods for <see cref="IEnumerable{T}"/> to find minimum and maximum elements and values.
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
    ///     Finds the minimum value produced by applying a valuation function to each element in the enumerator.
    /// </summary>
    /// <typeparam name="T">The type of elements in the enumerator.</typeparam>
    /// <typeparam name="TR">The comparable type returned by the valuation function.</typeparam>
    /// <param name="enumerator">The enumerator to traverse.</param>
    /// <param name="valuingFoo">The function that extracts a comparable value from each element.</param>
    /// <returns>The minimum value found in the sequence.</returns>
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
    ///     Finds the element whose valuation function produces the specified minimum value.
    /// </summary>
    /// <typeparam name="T">The type of elements in the enumerator.</typeparam>
    /// <typeparam name="TR">The comparable type returned by the valuation function.</typeparam>
    /// <param name="enumerator">The enumerator to traverse (will be reset).</param>
    /// <param name="valuingFoo">The function that extracts a comparable value from each element.</param>
    /// <param name="minValue">The minimum value to match against.</param>
    /// <returns>The first element whose valuation equals <paramref name="minValue"/>.</returns>
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
    ///     Finds the maximum value produced by applying a valuation function to each element in the enumerator.
    /// </summary>
    /// <typeparam name="T">The type of elements in the enumerator.</typeparam>
    /// <typeparam name="TR">The comparable type returned by the valuation function.</typeparam>
    /// <param name="enumerator">The enumerator to traverse.</param>
    /// <param name="valuingFoo">The function that extracts a comparable value from each element.</param>
    /// <returns>The maximum value found in the sequence.</returns>
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
    ///     Finds the element whose valuation function produces the specified maximum value.
    /// </summary>
    /// <typeparam name="T">The type of elements in the enumerator.</typeparam>
    /// <typeparam name="TR">The comparable type returned by the valuation function.</typeparam>
    /// <param name="enumerator">The enumerator to traverse (will be reset).</param>
    /// <param name="valuingFoo">The function that extracts a comparable value from each element.</param>
    /// <param name="maxValue">The maximum value to match against.</param>
    /// <returns>The first element whose valuation equals <paramref name="maxValue"/>.</returns>
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
    ///     Finds all elements whose valuation function produces the maximum value in the sequence.
    /// </summary>
    /// <typeparam name="T">The type of elements in the enumerator.</typeparam>
    /// <typeparam name="TR">The comparable type returned by the valuation function.</typeparam>
    /// <param name="enumerator">The enumerator to traverse.</param>
    /// <param name="valuingFoo">The function that extracts a comparable value from each element.</param>
    /// <returns>A list of all elements sharing the maximum value.</returns>
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
    ///     Finds all elements whose valuation function matches the specified maximum value.
    /// </summary>
    /// <typeparam name="T">The type of elements in the enumerator.</typeparam>
    /// <typeparam name="TR">The comparable type returned by the valuation function.</typeparam>
    /// <param name="enumerator">The enumerator to traverse (will be reset).</param>
    /// <param name="valuingFoo">The function that extracts a comparable value from each element.</param>
    /// <param name="maxValue">The maximum value to match against.</param>
    /// <returns>A list of all elements whose valuation equals <paramref name="maxValue"/>.</returns>
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
    ///     Collects all maximum values from the enumerator by comparing each element's valuation.
    /// </summary>
    /// <typeparam name="T">The type of elements in the enumerator.</typeparam>
    /// <typeparam name="TR">The comparable type returned by the valuation function.</typeparam>
    /// <param name="enumerator">The enumerator to traverse.</param>
    /// <param name="valuingFoo">The function that extracts a comparable value from each element.</param>
    /// <returns>A list of all values that share the maximum rank.</returns>
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
        ///     Returns all elements from the collection that share the maximum value as determined by the valuation function.
        /// </summary>
        /// <typeparam name="TR">The comparable type returned by the valuation function.</typeparam>
        /// <param name="valuingFoo">The function that extracts a comparable value from each element.</param>
        /// <returns>A <see cref="LanguageExt.Try{A}"/> containing the elements with the maximum value.</returns>
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
        ///     Returns the single element from the collection that has the maximum value as determined by the valuation function.
        /// </summary>
        /// <typeparam name="TR">The comparable type returned by the valuation function.</typeparam>
        /// <param name="valuingFoo">The function that extracts a comparable value from each element.</param>
        /// <returns>A <see cref="LanguageExt.Try{A}"/> containing the element with the maximum value.</returns>
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
        ///     Returns the single element from the collection that has the minimum value as determined by the valuation function.
        /// </summary>
        /// <typeparam name="TR">The comparable type returned by the valuation function.</typeparam>
        /// <param name="valuingFoo">The function that extracts a comparable value from each element.</param>
        /// <returns>A <see cref="LanguageExt.Try{A}"/> containing the element with the minimum value.</returns>
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