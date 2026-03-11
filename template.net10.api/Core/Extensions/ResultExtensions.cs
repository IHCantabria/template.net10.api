using template.net10.api.Core.Exceptions;

namespace template.net10.api.Core.Extensions;

/// <summary>
///     Provides extension methods for <see cref="LanguageExt.Common.Result{T}"/> to safely extract data or exceptions.
/// </summary>
internal static class ResultExtensions
{
    extension<T>(LanguageExt.Common.Result<T> result)
    {
        /// <summary>
        ///     Extracts the exception from a faulted result. Throws if the result is successful.
        /// </summary>
        /// <exception cref="ResultFaultedInvalidOperationException">
        ///     Result is not a failure! Use ExtractData method instead and
        ///     Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractData method
        /// </exception>
        /// <returns>The <see cref="Exception"/> contained in the faulted result.</returns>
        internal Exception ExtractException()
        {
            return result.Match(
                static _ => throw new ResultFaultedInvalidOperationException(
                    "Result is not a failure! Use ExtractData instead and check IsSuccess or IsFaulted before calling."),
                static ex => ex);
        }

        /// <summary>
        ///     Extracts the data value from a successful result. Throws if the result is faulted.
        /// </summary>
        /// <exception cref="ResultSuccessInvalidOperationException">
        ///     Result is not a success! Use ExtractException method instead
        ///     and Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractException method
        /// </exception>
        /// <returns>The data value of type <typeparamref name="T"/> contained in the successful result.</returns>
        internal T ExtractData()
        {
            return result.Match(static data => data, static _ => throw new ResultSuccessInvalidOperationException(
                "Result is not a success! Use ExtractException instead and check IsSuccess or IsFaulted before calling."));
        }
    }
}