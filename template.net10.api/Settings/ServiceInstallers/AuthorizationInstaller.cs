using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using template.net10.api.Core.Authorization;
using template.net10.api.Settings.Extensions;
using template.net10.api.Settings.Interfaces;

namespace template.net10.api.Settings.ServiceInstallers;

/// <summary>
///     Service installer that registers the <see cref="ClaimRequirementHandler" /> and all application
///     authorization policies. Policies are environment-aware: production uses strict AND logic,
///     non-production uses lenient OR logic. Load order: 19.
/// </summary>
[UsedImplicitly]
internal sealed class AuthorizationInstaller : IServiceInstaller
{
    /// <inheritdoc cref="IServiceInstaller.LoadOrder" />
    public short LoadOrder => 19;

    /// <inheritdoc cref="IServiceInstaller.InstallServiceAsync" />
    /// <exception cref="ArgumentNullException"><paramref name="builder" /> is <see langword="null" />.</exception>
    public Task InstallServiceAsync(WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.AddSingleton<IAuthorizationHandler, ClaimRequirementHandler>();

        var authorizationBuilder = builder.Services.AddAuthorizationBuilder();
        var isProduction = builder.Environment.EnvironmentName == Envs.Production;

        authorizationBuilder.AddPolicies(isProduction);

        return Task.CompletedTask;
    }
}