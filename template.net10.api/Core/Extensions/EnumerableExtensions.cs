using System.Diagnostics.CodeAnalysis;
using LanguageExt;
using template.net10.api.Core.Exceptions;

namespace template.net10.api.Core.Extensions;

/// <summary>
///     Provides extension methods for <see cref="IEnumerable{T}" /> to find minimum and maximum elements and values.
/// </summary>
[SuppressMessage(
    "ReSharper",
    "UnusedMember.Local",
    Justification = "General-purpose helper methods; not all members are used in every scenario.")]
file static class EnumerableExtensions
{
    extension<T>(IEnumerable<T> container)
    {
        /// <summary>
        ///     Returns all elements from the collection that share the maximum value as determined by the valuation function.
        /// </summary>
        /// <typeparam name="TResult">The comparable type returned by the valuation function.</typeparam>
        /// <param name="valuingFoo">The function that extracts a comparable value from each element.</param>
        /// <returns>A <see cref="LanguageExt.Try{A}" /> containing the elements with the maximum value.</returns>
        /// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created.</exception>
        internal Try<IEnumerable<T>> MaxElements<TResult>(Func<T, TResult> valuingFoo)
            where TResult : IComparable
        {
            return () =>
            {
                using var enumerator = container.GetEnumerator();

                return !enumerator.MoveNext()
                    ? new LanguageExt.Common.Result<IEnumerable<T>>(new ArgumentException("Container is empty!"))
                    : IEnumerable<T>.FindMaxElements(enumerator, valuingFoo);
            };
        }

        /// <summary>
        ///     Finds all elements whose valuation function produces the maximum value in the sequence.
        /// </summary>
        private static List<TValue> FindMaxElements<TValue, TResult>(IEnumerator<TValue> enumerator,
            Func<TValue, TResult> valuingFoo)
            where TResult : IComparable
        {
            var maxValues = IEnumerable<T>.FindMaxValues(enumerator, valuingFoo);
            return IEnumerable<T>.FindMaxElements(enumerator, valuingFoo, maxValues[0]);
        }

        /// <summary>
        ///     Finds all elements whose valuation function matches the specified maximum value.
        /// </summary>
        private static List<TValue> FindMaxElements<TValue, TResult>(IEnumerator<TValue> enumerator,
            Func<TValue, TResult> valuingFoo, TResult maxValue)
            where TResult : IComparable
        {
            var maxElements = new List<TValue>();

            enumerator.Reset();

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
        private static List<TResult> FindMaxValues<TValue, TResult>(IEnumerator<TValue> enumerator,
            Func<TValue, TResult> valuingFoo)
            where TResult : IComparable
        {
            var maxValues = new List<TResult> { valuingFoo(enumerator.Current) };

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

        /// <summary>
        ///     Returns the single element from the collection that has the maximum value as determined by the valuation function.
        /// </summary>
        /// <typeparam name="TResult">The comparable type returned by the valuation function.</typeparam>
        /// <param name="valuingFoo">The function that extracts a comparable value from each element.</param>
        /// <returns>A <see cref="LanguageExt.Try{A}" /> containing the element with the maximum value.</returns>
        /// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created.</exception>
        /// <exception cref="NotSupportedException">The enumerator does not support being reset.</exception>
        /// <exception cref="ResultSuccessInvalidOperationException">
        ///     Result is not a success! Use ExtractException method instead
        ///     and Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractException method
        /// </exception>
        /// <exception cref="ResultFaultedInvalidOperationException">
        ///     Result is not a failure! Use ExtractData method instead and
        ///     Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractData method
        /// </exception>
        internal Try<T> MaxElement<TResult>(Func<T, TResult> valuingFoo)
            where TResult : IComparable
        {
            return () =>
            {
                using var enumerator = container.GetEnumerator();

                var maxValueResult = IEnumerable<T>.FindMaxValue(enumerator, valuingFoo).Try();
                if (maxValueResult.IsFaulted)
                    return new LanguageExt.Common.Result<T>(maxValueResult.ExtractException());

                enumerator.Reset();
                return IEnumerable<T>.FindMaxElement(enumerator, valuingFoo, maxValueResult.ExtractData()).Try();
            };
        }

        /// <summary>
        ///     Finds the element whose valuation function produces the specified maximum value.
        /// </summary>
        private static Try<TValue> FindMaxElement<TValue, TResult>(IEnumerator<TValue> enumerator,
            Func<TValue, TResult> valuingFoo,
            TResult maxValue)
            where TResult : IComparable
        {
            return () =>
            {
                enumerator.Reset();

                while (enumerator.MoveNext())
                {
                    var currentVal = valuingFoo(enumerator.Current);
                    if (currentVal.CompareTo(maxValue) == 0) return enumerator.Current;
                }

                return new LanguageExt.Common.Result<TValue>(
                    new ArgumentException("Enumerator is empty!"));
            };
        }

        /// <summary>
        ///     Finds the maximum value produced by applying a valuation function to each element in the enumerator.
        /// </summary>
        private static Try<TResult> FindMaxValue<TValue, TResult>(IEnumerator<TValue> enumerator,
            Func<TValue, TResult> valuingFoo)
            where TResult : IComparable
        {
            return () =>
            {
                if (!enumerator.MoveNext())
                    return new LanguageExt.Common.Result<TResult>(
                        new ArgumentException("Container is empty!"));

                var maxVal = valuingFoo(enumerator.Current);

                while (enumerator.MoveNext())
                {
                    var currentVal = valuingFoo(enumerator.Current);
                    if (currentVal.CompareTo(maxVal) > 0) maxVal = currentVal;
                }

                return maxVal;
            };
        }

        /// <summary>
        ///     Returns the single element from the collection that has the minimum value as determined by the valuation function.
        /// </summary>
        /// <typeparam name="TResult">The comparable type returned by the valuation function.</typeparam>
        /// <param name="valuingFoo">The function that extracts a comparable value from each element.</param>
        /// <returns>A <see cref="LanguageExt.Try{A}" /> containing the element with the minimum value.</returns>
        /// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created.</exception>
        /// <exception cref="NotSupportedException">The enumerator does not support being reset.</exception>
        /// <exception cref="ResultSuccessInvalidOperationException">
        ///     Result is not a success! Use ExtractException method instead
        ///     and Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractException method
        /// </exception>
        /// <exception cref="ResultFaultedInvalidOperationException">
        ///     Result is not a failure! Use ExtractData method instead and
        ///     Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractData method
        /// </exception>
        internal Try<T> MinElement<TResult>(Func<T, TResult> valuingFoo)
            where TResult : IComparable
        {
            return () =>
            {
                using var enumerator = container.GetEnumerator();

                var minValueResult = IEnumerable<T>.FindMinValue(enumerator, valuingFoo).Try();
                if (minValueResult.IsFaulted)
                    return new LanguageExt.Common.Result<T>(minValueResult.ExtractException());

                enumerator.Reset();
                return IEnumerable<T>.FindMinElement(enumerator, valuingFoo, minValueResult.ExtractData()).Try();
            };
        }

        /// <summary>
        ///     Finds the minimum value produced by applying a valuation function to each element in the enumerator.
        /// </summary>
        private static Try<TResult> FindMinValue<TValue, TResult>(IEnumerator<TValue> enumerator,
            Func<TValue, TResult> valuingFoo)
            where TResult : IComparable
        {
            return () =>
            {
                if (!enumerator.MoveNext())
                    return new LanguageExt.Common.Result<TResult>(
                        new ArgumentException("Container is empty!"));

                var minVal = valuingFoo(enumerator.Current);

                while (enumerator.MoveNext())
                {
                    var currentVal = valuingFoo(enumerator.Current);
                    if (currentVal.CompareTo(minVal) < 0) minVal = currentVal;
                }

                return minVal;
            };
        }

        /// <summary>
        ///     Finds the element whose valuation function produces the specified minimum value.
        /// </summary>
        private static Try<TValue> FindMinElement<TValue, TResult>(IEnumerator<TValue> enumerator,
            Func<TValue, TResult> valuingFoo,
            TResult minValue)
            where TResult : IComparable
        {
            return () =>
            {
                enumerator.Reset();

                while (enumerator.MoveNext())
                {
                    var currentVal = valuingFoo(enumerator.Current);
                    if (currentVal.CompareTo(minValue) == 0) return enumerator.Current;
                }

                return new LanguageExt.Common.Result<TValue>(
                    new InvalidOperationException("Minimum element not found!"));
            };
        }
    }
}