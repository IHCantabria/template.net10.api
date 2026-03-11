namespace template.net10.api.Core.Attributes;

/// <summary>
///     Specifies the dependency injection service lifetime for a class.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
internal sealed class ServiceLifetimeAttribute(ServiceLifetime serviceLifetime) : Attribute
{
    /// <summary>
    ///     Gets the dependency injection service lifetime.
    /// </summary>
    public ServiceLifetime ServiceLifetime { get; } = serviceLifetime;
}