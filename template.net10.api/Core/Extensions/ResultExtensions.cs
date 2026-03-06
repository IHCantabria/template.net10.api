using template.net10.api.Core.Exceptions;

namespace template.net10.api.Core.Extensions;

/// <summary>
///     ADD DOCUMENTATION
/// </summary>
internal static class ResultExtensions
{
    extension<T>(LanguageExt.Common.Result<T> result)
    {
        /// <summary>
        ///     ADD DOCUMENTATION
        /// </summary>
        /// <exception cref="ResultFaultedInvalidOperationException">
        ///     Result is not a failure! Use ExtractData method instead and
        ///     Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractData method
        /// </exception>
        internal Exception ExtractException()
        {
            return result.Match(
                static _ => throw new ResultFaultedInvalidOperationException(
                    "Result is not a failure! Use ExtractData instead and check IsSuccess or IsFaulted before calling."),
                static ex => ex);
        }

        /// <summary>
        ///     ADD DOCUMENTATION
        /// </summary>
        /// <exception cref="ResultSuccessInvalidOperationException">
        ///     Result is not a success! Use ExtractException method instead
        ///     and Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractException method
        /// </exception>
        internal T ExtractData()
        {
            return result.Match(static data => data, static _ => throw new ResultSuccessInvalidOperationException(
                "Result is not a success! Use ExtractException instead and check IsSuccess or IsFaulted before calling."));
        }
    }
}