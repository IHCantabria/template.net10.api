using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace template.net10.api.Core.Json;

/// <summary>
///     JSON converter that transforms string values to camelCase during serialization and deserialization.
/// </summary>
internal sealed class CamelCaseStringConverter : JsonConverter<string>
{
    /// <summary>
    ///     Reads a JSON string token and returns its value converted to camelCase.
    /// </summary>
    /// <param name="reader">The UTF-8 JSON reader to read from.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">The serializer options.</param>
    /// <returns>The camelCase string value, or <see langword="null"/> for a JSON null token.</returns>
    /// <exception cref="JsonException">Expected JSON string</exception>
    /// <exception cref="InvalidOperationException">
    ///     The JSON token value isn't a string (that is, not a <see cref="System.Text.Json.JsonTokenType.String" />,
    ///     <see cref="System.Text.Json.JsonTokenType.PropertyName" />, or <see cref="System.Text.Json.JsonTokenType.Null" />).
    ///     -or-
    ///     The JSON string contains invalid UTF-8 bytes or invalid UTF-16 surrogates.
    /// </exception>
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        return reader.TokenType != JsonTokenType.String
            ? throw new JsonException("Expected JSON string")
            : ToCamelCase(reader.GetString());
    }

    /// <summary>
    ///     Writes a string value converted to camelCase into the JSON output.
    /// </summary>
    /// <param name="writer">The UTF-8 JSON writer to write to.</param>
    /// <param name="value">The string value to write in camelCase.</param>
    /// <param name="options">The serializer options.</param>
    /// <exception cref="InvalidOperationException">
    ///     Validation is enabled, and the operation would result in writing invalid
    ///     JSON.
    /// </exception>
    /// <exception cref="ArgumentException">The specified value is too large.</exception>
    public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(ToCamelCase(value));
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