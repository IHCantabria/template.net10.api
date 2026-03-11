using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace template.net10.api.Core.Json;

/// <summary>
///     JSON converter that transforms string collections to camelCase during serialization and deserialization.
/// </summary>
internal sealed class CamelCaseStringEnumerableConverter : JsonConverter<IEnumerable<string>>
{
    /// <summary>
    ///     Reads a JSON array of strings and returns them converted to camelCase.
    /// </summary>
    /// <param name="reader">The UTF-8 JSON reader to read from.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">The serializer options.</param>
    /// <returns>The collection of camelCase string values.</returns>
    /// <exception cref="JsonException">Thrown when the current token is not the start of an array.</exception>
    public override IEnumerable<string> Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        ValidateArrayStart(reader);
        return ReadStringArray(ref reader);
    }

    /// <summary>
    ///     Validates that the current JSON token is the start of an array.
    /// </summary>
    /// <param name="reader">The UTF-8 JSON reader to validate.</param>
    /// <exception cref="JsonException">Thrown when the current token is not the start of an array.</exception>
    private static void ValidateArrayStart(Utf8JsonReader reader)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException("Expected JSON array");
    }

    /// <summary>
    ///     Reads all string elements from a JSON array and converts each to camelCase.
    /// </summary>
    /// <param name="reader">The UTF-8 JSON reader positioned at the start of the array.</param>
    /// <returns>A list of camelCase string values read from the array.</returns>
    [SuppressMessage(
        "ReSharper",
        "ReturnTypeCanBeEnumerable.Local",
        Justification =
            "Concrete return type is intentional for performance and to avoid interface-based enumeration overhead.")]
    private static List<string> ReadStringArray(ref Utf8JsonReader reader)
    {
        var result = new List<string>();

        // Move past the start array token
        reader.Read();

        while (reader.TokenType != JsonTokenType.EndArray)
        {
            result.Add(ReadArrayElement(ref reader) ?? throw new InvalidOperationException());
            reader.Read();
        }

        return result;
    }

    /// <summary>
    ///     Reads a single array element and converts its string value to camelCase.
    /// </summary>
    /// <param name="reader">The UTF-8 JSON reader positioned at the current element.</param>
    /// <returns>The camelCase string value, or <see langword="null"/> for a JSON null token.</returns>
    /// <exception cref="JsonException">Thrown when the token is neither a string nor null.</exception>
    [SuppressMessage(
        "ReSharper",
        "SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault",
        Justification = "Default case intentionally throws for unsupported JSON token types.")]
    private static string? ReadArrayElement(ref Utf8JsonReader reader)
    {
        return reader.TokenType switch
        {
            JsonTokenType.String => ToCamelCase(reader.GetString()),
            JsonTokenType.Null => null,
            _ => throw new JsonException($"Unexpected token type: {reader.TokenType}")
        };
    }

    /// <summary>
    ///     Writes a collection of strings as a JSON array with each value converted to camelCase.
    /// </summary>
    /// <param name="writer">The UTF-8 JSON writer to write to.</param>
    /// <param name="value">The collection of strings to write as a camelCase JSON array.</param>
    /// <param name="options">The serializer options.</param>
    /// <exception cref="InvalidOperationException">
    ///     Validation is enabled, and the operation would result in writing invalid
    ///     JSON.
    /// </exception>
    /// <exception cref="ArgumentException">The specified value is too large.</exception>
    public override void Write(Utf8JsonWriter writer, IEnumerable<string> value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartArray();

        foreach (var item in value)
            if (item is null)
                writer.WriteNullValue();
            else
                writer.WriteStringValue(ToCamelCase(item));

        writer.WriteEndArray();
    }

    /// <summary>
    ///     Converts a string to camelCase by lowering its first character.
    /// </summary>
    /// <param name="str">The string to convert.</param>
    /// <returns>The camelCase string, or <see langword="null"/> if the input is null.</returns>
    [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase",
        Justification = "Lowercase normalization is required to implement camelCase naming rules.")]
    private static string? ToCamelCase(string? str)
    {
        // Handle special cases
        if (string.IsNullOrEmpty(str) || str.Length == 1)
            return str?.ToLowerInvariant();

        // Optimization to avoid creating a new string if already in camelCase
        if (char.IsLower(str[0]))
            return str;

        // Convert the first character to lowercase and keep the rest unchanged
        return char.ToLowerInvariant(str[0]) + str[1..];
    }
}