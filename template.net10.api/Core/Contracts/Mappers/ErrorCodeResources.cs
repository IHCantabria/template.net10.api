using Microsoft.Extensions.Localization;

namespace template.net10.api.Core.Contracts;

public sealed partial record ErrorCodeResource
{
    /// <summary>
    ///     Implicitly converts a <see cref="LocalizedString" /> to an <see cref="ErrorCodeResource" />.
    /// </summary>
    /// <param name="obj">The localized string to convert.</param>
    /// <returns>A new <see cref="ErrorCodeResource" /> with the key and description from the localized string.</returns>
    public static implicit operator ErrorCodeResource(LocalizedString obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        return new ErrorCodeResource
        {
            Key = obj.Name,
            Description = obj.Value
        };
    }

    /// <summary>
    ///     Converts a <see cref="LocalizedString" /> to an <see cref="ErrorCodeResource" />.
    /// </summary>
    /// <param name="obj">The localized string to convert.</param>
    /// <returns>A new <see cref="ErrorCodeResource" /> instance.</returns>
    public static ErrorCodeResource ToErrorCodeResource(
        LocalizedString obj)
    {
        return obj;
    }

    /// <summary>
    ///     Converts a read-only list of <see cref="LocalizedString" /> instances to a collection of
    ///     <see cref="ErrorCodeResource" />.
    /// </summary>
    /// <param name="objs">The read-only list of localized strings to convert.</param>
    /// <returns>A collection of <see cref="ErrorCodeResource" /> instances.</returns>
    internal static IEnumerable<ErrorCodeResource> ToCollection(
        IReadOnlyList<LocalizedString> objs)
    {
        var resources = new ErrorCodeResource[objs.Count];
        for (var i = 0; i < objs.Count; i++) resources[i] = objs[i];
        return resources;
    }
}