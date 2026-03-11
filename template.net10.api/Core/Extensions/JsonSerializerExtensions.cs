using System.Text.Json;
using System.Text.Json.Serialization;

namespace template.net10.api.Core.Extensions;

/// <summary>
///     Provides extension methods for <see cref="JsonSerializerOptions"/> to configure core serialization settings.
/// </summary>
internal static class JsonSerializerExtensions
{
    extension(JsonSerializerOptions options)
    {
        /// <summary>
        ///     Applies core JSON serialization options including cycle handling, named floating-point literals, and trailing commas.
        /// </summary>
        /// <exception cref="InvalidOperationException">This property is set after serialization or deserialization has occurred.</exception>
        /// <returns>The configured <see cref="JsonSerializerOptions"/> instance for method chaining.</returns>
        internal JsonSerializerOptions AddCoreOptions()
        {
            options.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            options.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
            options.AllowTrailingCommas = true;
            return options;
        }
    }
}