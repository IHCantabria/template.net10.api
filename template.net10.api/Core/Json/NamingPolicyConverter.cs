using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using template.net10.api.Core.Extensions;

namespace template.net10.api.Core.Json;

/// <summary>
///     JSON converter that dynamically selects a naming policy (camelCase, snake_case, kebab-case) based on HTTP request
///     headers.
/// </summary>
internal sealed class NamingPolicyConverter(IHttpContextAccessor httpContextAccessor) : JsonConverter<object>
{
    /// <summary>
    ///     The HTTP context accessor used to retrieve naming policy headers from the current request.
    /// </summary>
    private readonly IHttpContextAccessor _httpContextAccessor =
        httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

    /// <summary>
    ///     Returns <see langword="true" /> for all types, indicating this converter handles any object type.
    /// </summary>
    /// <param name="typeToConvert">The type to check conversion support for.</param>
    /// <returns><see langword="true"/> for all types; this converter handles any object type.</returns>
    public override bool CanConvert(Type typeToConvert)
    {
        return true;
    }

    /// <summary>
    ///     Reads a JSON value using the naming policy specified in the <c>x-json-input-naming-policy</c> request header.
    /// </summary>
    /// <param name="reader">The UTF-8 JSON reader to read from.</param>
    /// <param name="typeToConvert">The target type to deserialize into.</param>
    /// <param name="options">The serializer options (naming policy is overridden by the <c>x-json-input-naming-policy</c> header).</param>
    /// <returns>The deserialized object with naming policy applied from the <c>x-json-input-naming-policy</c> header.</returns>
    /// <exception cref="JsonException">A value could not be read from the reader.</exception>
    /// <exception cref="ArgumentException">
    ///     <paramref name="reader" /> contains unsupported options.
    ///     -or-
    ///     The current <paramref name="reader" /> token does not start or represent a value.
    /// </exception>
    /// <exception cref="InvalidOperationException">This property is set after serialization or deserialization has occurred.</exception>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        var headerValue = request?.Headers["x-json-input-naming-policy"].ToString();
        var namingPolicy = GetNamingPolicyFromHeader(headerValue);
        options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = namingPolicy
        };
        options.AddCoreOptions();

        using var doc = JsonDocument.ParseValue(ref reader);
        return JsonSerializer.Deserialize(doc.RootElement.GetRawText(), typeToConvert, options);
    }

    /// <summary>
    ///     Writes a JSON value using the naming policy specified in the <c>x-json-output-naming-policy</c> request header.
    /// </summary>
    /// <param name="writer">The UTF-8 JSON writer to write to.</param>
    /// <param name="value">The object to serialize.</param>
    /// <param name="options">The serializer options (naming policy is overridden by the <c>x-json-output-naming-policy</c> header).</param>
    /// <exception cref="ArgumentNullException"><paramref name="writer" /> is <see langword="null" />.</exception>
    /// <exception cref="InvalidOperationException">This property is set after serialization or deserialization has occurred.</exception>
    [SuppressMessage(
        "ReSharper",
        "ExceptionNotDocumentedOptional",
        Justification =
            "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        var headerValue = request?.Headers["x-json-output-naming-policy"].ToString();
        var namingPolicy = GetNamingPolicyFromHeader(headerValue);

        options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = namingPolicy
        };
        options.AddCoreOptions();

        JsonSerializer.Serialize(writer, value, options);
    }

    /// <summary>
    ///     Resolves a <see cref="JsonNamingPolicy" /> from the header value. Defaults to snake_case.
    /// </summary>
    /// <param name="headerValue">The raw header value (<c>"camel"</c>, <c>"snake"</c>, or <c>"kebab"</c>); defaults to snake_case if <see langword="null"/> or unrecognized.</param>
    /// <returns>The resolved <see cref="JsonNamingPolicy"/>.</returns>
    private static JsonNamingPolicy GetNamingPolicyFromHeader(string? headerValue)
    {
        if (string.Equals(headerValue, "camel", StringComparison.OrdinalIgnoreCase))
            return JsonNamingPolicy.CamelCase;
        if (string.Equals(headerValue, "snake", StringComparison.OrdinalIgnoreCase))
            return JsonNamingPolicy.SnakeCaseLower;

        return string.Equals(headerValue, "kebab", StringComparison.OrdinalIgnoreCase)
            ? JsonNamingPolicy.KebabCaseLower
            : JsonNamingPolicy.SnakeCaseLower; // Default to Snake if header is not present or not recognized
    }
}