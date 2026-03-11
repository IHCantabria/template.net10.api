using System.Reflection;
using JetBrains.Annotations;
using template.net10.api.Core.Attributes;
using template.net10.api.Core.Interfaces;
using template.net10.api.Settings.Interfaces;
using ZLinq;
using ZLinq.Linq;

namespace template.net10.api.Settings.ServiceInstallers;

/// <summary>
///     Service installer that auto-discovers and registers all <see cref="IServiceImplementation" /> types
///     from the application assembly, mapping each concrete class to its matching interface using
///     the convention <c>I{ClassName}</c>. The DI lifetime is controlled by
///     <see cref="ServiceLifetimeAttribute" />, defaulting to <see cref="ServiceLifetime.Scoped" />. Load order: 11.
/// </summary>
[UsedImplicitly]
internal sealed class ServicesInstaller : IServiceInstaller
{
    /// <inheritdoc cref="IServiceInstaller.LoadOrder" />
    public short LoadOrder => 11;

    /// <inheritdoc cref="IServiceInstaller.InstallServiceAsync" />
    /// <exception cref="ArgumentNullException"><paramref name="builder" /> is <see langword="null" />.</exception>
    public Task InstallServiceAsync(WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        //Should be Serial
        foreach (var serviceType in GetExportedServiceTypes())
        {
            var interfaceType = GetInterfaceType(serviceType);

            if (interfaceType is not null) RegisterService(builder.Services, serviceType, interfaceType);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Reflects over the application assembly to find all non-abstract, non-interface types
    ///     that implement <see cref="IServiceImplementation" />.
    /// </summary>
    /// <returns>An enumerable of concrete service implementation types.</returns>
    private static ValueEnumerable<ArrayWhere<Type>, Type> GetExportedServiceTypes()
    {
        return typeof(Program).Assembly
            .GetTypes()
            .Where(static t => typeof(IServiceImplementation).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
    }

    /// <summary>
    ///     Finds the interface type that follows the <c>I{ClassName}</c> naming convention
    ///     among the interfaces implemented by <paramref name="serviceType" />.
    /// </summary>
    /// <param name="serviceType">The concrete service type to inspect.</param>
    /// <returns>The matching interface type, or <see langword="null" /> if none found.</returns>
    private static Type? GetInterfaceType(Type serviceType)
    {
        return serviceType.GetInterfaces().SingleOrDefault(i => i.Name == $"I{serviceType.Name}");
    }

    /// <summary>
    ///     Registers <paramref name="serviceType" /> against <paramref name="interfaceType" /> in the
    ///     DI container with the lifetime declared by <see cref="ServiceLifetimeAttribute" />,
    ///     defaulting to <see cref="ServiceLifetime.Scoped" /> when no attribute is present.
    /// </summary>
    /// <param name="services">The application service collection to register into.</param>
    /// <param name="serviceType">The concrete implementation type.</param>
    /// <param name="interfaceType">The service interface type to register against.</param>
    private static void RegisterService(IServiceCollection services, Type serviceType, Type interfaceType)
    {
        var serviceLifetimeAttribute =
            serviceType
                .GetCustomAttributes<ServiceLifetimeAttribute>().FirstOrDefault();

        switch (serviceLifetimeAttribute?.ServiceLifetime ?? ServiceLifetime.Scoped)
        {
            case ServiceLifetime.Scoped:
                services.AddScoped(interfaceType, serviceType);
                break;
            case ServiceLifetime.Transient:
                services.AddTransient(interfaceType, serviceType);
                break;
            case ServiceLifetime.Singleton:
                services.AddSingleton(interfaceType, serviceType);
                break;
            default:
                services.AddScoped(interfaceType, serviceType);
                break;
        }
    }
}