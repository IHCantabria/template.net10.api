using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using LanguageExt;
using template.net10.api.Core.Exceptions;
using template.net10.api.Core.Extensions;

namespace template.net10.api.Core.Json;

/// <summary>
///     Provides deep cloning capabilities for objects using JSON serialization and deserialization.
/// </summary>
[SuppressMessage(
    "ReSharper",
    "UnusedType.Global",
    Justification = "General-purpose helper type; usage depends on consumer requirements.")]
[SuppressMessage(
    "ReSharper",
    "UnusedMember.Global",
    Justification = "General-purpose helper methods; not all members are used in every scenario.")]
internal static class Cloner
{
    /// <summary>
    ///     Default JSON serializer options configured with core application settings.
    /// </summary>
    private static readonly JsonSerializerOptions Options = new JsonSerializerOptions().AddCoreOptions();

    /// <summary>
    ///     Creates a deep clone of the specified object by serializing and deserializing it as JSON.
    /// </summary>
    /// <typeparam name="T">The type of the object to clone.</typeparam>
    /// <param name="obj">The object to deep clone.</param>
    /// <returns>A <see cref="LanguageExt.Try{T}"/> containing the cloned object or an exception.</returns>
    /// <exception cref="ResultSuccessInvalidOperationException">
    ///     Result is not a success! Use ExtractException method instead
    ///     and Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractException method
    /// </exception>
    /// <exception cref="ResultFaultedInvalidOperationException">
    ///     Result is not a failure! Use ExtractData method instead and
    ///     Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractData method
    /// </exception>
    internal static Try<T> DeepClone<T>(T obj)
    {
        return () =>
        {
            var serializeResult = Serialize(obj).Try();
            return serializeResult.IsSuccess
                ? Deserialize<T>(serializeResult.ExtractData()).Try()
                : new LanguageExt.Common.Result<T>(serializeResult.ExtractException());
        };
    }

    /// <summary>
    ///     Serializes an object to its JSON string representation.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="obj">The object to serialize.</param>
    /// <returns>A <see cref="LanguageExt.Try{T}"/> containing the JSON string or an exception.</returns>
    private static Try<string> Serialize<T>(T obj)
    {
        return () => JsonSerializer.Serialize(obj, Options);
    }

    /// <summary>
    ///     Deserializes a JSON string back into an object of the specified type.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the JSON string into.</typeparam>
    /// <param name="stringObj">The JSON string to deserialize.</param>
    /// <returns>A <see cref="LanguageExt.Try{T}"/> containing the deserialized object or an exception.</returns>
    private static Try<T> Deserialize<T>(string stringObj)
    {
        return () =>
        {
            var obj = JsonSerializer.Deserialize<T>(stringObj, Options);
            return obj is not null
                ? obj
                : new LanguageExt.Common.Result<T>(
                    new CoreException("The response received from the Deserializer is empty"));
        };
    }
}