using System.Text.Json;
using System.Text.Json.Serialization;

namespace template.net10.api.Core.Extensions;

/// <summary>
///     ADD DOCUMENTATION
/// </summary>
internal static class JsonSerializerExtensions
{
    extension(JsonSerializerOptions options)
    {
        /// <summary>
        ///     ADD DOCUMENTATION
        /// </summary>
        /// <exception cref="InvalidOperationException">This property is set after serialization or deserialization has occurred.</exception>
        internal JsonSerializerOptions AddCoreOptions()
        {
            options.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            options.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
            options.AllowTrailingCommas = true;
            return options;
        }
    }
}