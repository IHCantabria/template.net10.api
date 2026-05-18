namespace template.net10.api.Core.Attributes;

/// <summary>
///     Specifies the dependency injection service type for a class.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
internal sealed class ServiceTypeAttribute(ServiceType serviceType) : Attribute
{
    /// <summary>
    ///     Gets the dependency injection service type.
    /// </summary>
    public ServiceType ServiceType { get; } = serviceType;
}

/// <summary>
///     Defines the dependency injection registration category for a service class.
///     Used by <see cref="ServiceTypeAttribute" /> to indicate whether the annotated class
///     should be registered as a standard service or as a hosted background service.
/// </summary>
internal enum ServiceType
{
    /// <summary>
    ///     Indicates that the service is a standard application service (e.g. scoped, transient or singleton)
    ///     registered through the normal DI container.
    /// </summary>
    Normal = 0,

    /// <summary>
    ///     Indicates that the service is a long-running hosted background service derived from
    ///     <see cref="Microsoft.Extensions.Hosting.BackgroundService" />, registered via
    ///     <c>AddHostedService</c>.
    /// </summary>
    Background = 1
}