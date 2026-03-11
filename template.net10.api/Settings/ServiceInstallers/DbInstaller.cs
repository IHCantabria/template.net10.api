using EntityFramework.Exceptions.PostgreSQL;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Protocols.Configuration;
using Npgsql;
using template.net10.api.Core;
using template.net10.api.Core.Timeout;
using template.net10.api.Persistence.Context;
using template.net10.api.Settings.Interfaces;
using template.net10.api.Settings.Options;

namespace template.net10.api.Settings.ServiceInstallers;

/// <summary>
///     Service installer that registers a pooled <see cref="AppDbContext" /> factory backed by Npgsql,
///     with environment-sensitive logging, retry policies, and PostGIS spatial support. Load order: 8.
/// </summary>
[UsedImplicitly]
internal sealed class DbInstaller : IServiceInstaller
{
    /// <inheritdoc cref="IServiceInstaller.LoadOrder" />
    public short LoadOrder => 8;

    /// <inheritdoc cref="IServiceInstaller.InstallServiceAsync" />
    /// <exception cref="ArgumentNullException"><paramref name="builder" /> is <see langword="null" />.</exception>
    /// <exception cref="InvalidConfigurationException">
    ///     The App Db configuration in the appsettings file is incorrect.
    /// </exception>
    public Task InstallServiceAsync(WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Configure strongly typed options objects
        var connectionOptions = builder.Configuration
            .GetSection(AppDbOptions.AppDb)
            .Get<AppDbOptions>();

        OptionsValidator.ValidateAppDbOptions(connectionOptions);
        AddDbContextPool(builder, connectionOptions);
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Registers the pooled <see cref="AppDbContext" /> factory, a transient <see cref="AppDbContextFactory" />,
    ///     and a scoped direct <see cref="AppDbContext" />. Also registers the developer-page exception filter
    ///     in non-production environments. Does nothing when no connection string is configured.
    /// </summary>
    /// <param name="builder">The application host builder.</param>
    /// <param name="connectionOptions">The resolved database options, or <see langword="null" /> if not configured.</param>
    private static void AddDbContextPool(IHostApplicationBuilder builder, AppDbOptions? connectionOptions)
    {
        // Register a pooling context factory as a Singleton service
        if (connectionOptions is null || string.IsNullOrEmpty(connectionOptions.ConnectionString)) return;

        builder.Services.AddPooledDbContextFactory<AppDbContext>(options =>
            ConfigureDbContext(options, builder, connectionOptions));
        // Register an additional context factory as a Transient service, which gets a pooled context from the Singleton factory we registered above
        builder.Services.AddTransient<AppDbContextFactory>();
        //Add a scoped service to create a new context instance, used for direct AppDBContext access
        builder.Services.AddScoped(static sp =>
            sp.GetRequiredService<IDbContextFactory<AppDbContext>>().CreateDbContext());
        if (builder.Environment.EnvironmentName is Envs.Development or Envs.Local or Envs.Test)
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
    }

    /// <summary>
    ///     Entry point for building the full <see cref="DbContextOptionsBuilder" /> configuration:
    ///     development settings, diagnostic warnings, and the database provider.
    ///     Skips configuration when the builder is already configured.
    /// </summary>
    /// <param name="options">The EF Core options builder to configure.</param>
    /// <param name="builder">The host application builder providing environment and services.</param>
    /// <param name="connectionOptions">The resolved database connection options.</param>
    private static void ConfigureDbContext(DbContextOptionsBuilder options, IHostApplicationBuilder builder,
        AppDbOptions connectionOptions)
    {
        if (options.IsConfigured) return;

        ConfigureDevelopmentSettings(options, builder);
        ConfigureWarnings(options);
        ConfigureDatabaseProvider(options, builder, connectionOptions);
    }

    /// <summary>
    ///     Enables EF Core sensitive data logging and detailed errors only in Development, Local, or Test environments.
    /// </summary>
    /// <param name="options">The EF Core options builder to configure.</param>
    /// <param name="builder">The host application builder providing the current environment name.</param>
    private static void ConfigureDevelopmentSettings(DbContextOptionsBuilder options, IHostApplicationBuilder builder)
    {
        if (builder.Environment.EnvironmentName is not (Envs.Development or Envs.Local or Envs.Test)) return;

        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
        options.ConfigureWarnings(static w =>
            w.Ignore(CoreEventId.SensitiveDataLoggingEnabledWarning));
    }

    /// <summary>
    ///     Configures EF Core to throw on multi-collection-include queries,
    ///     preventing accidental Cartesian-explosion performance issues.
    /// </summary>
    /// <param name="options">The EF Core options builder to configure.</param>
    private static void ConfigureWarnings(DbContextOptionsBuilder options)
    {
        options.ConfigureWarnings(static w =>
            w.Throw(RelationalEventId.MultipleCollectionIncludeWarning));
    }

    /// <summary>
    ///     Registers Npgsql as the EF Core database provider with PostGIS/NetTopologySuite support,
    ///     a command timeout, and an exponential retry policy for transient failures.
    /// </summary>
    /// <param name="options">The EF Core options builder to configure.</param>
    /// <param name="builder">The host application builder providing environment information.</param>
    /// <param name="connectionOptions">The database connection options containing the connection string.</param>
    private static void ConfigureDatabaseProvider(
        DbContextOptionsBuilder options,
        IHostApplicationBuilder builder,
        AppDbOptions connectionOptions)
    {
        options.UseNpgsql(
                GetNpgsqlDataSource(builder.Environment, connectionOptions), static npgsqlOptions =>
                {
                    npgsqlOptions.UseNetTopologySuite();
                    npgsqlOptions.CommandTimeout(DbContextConstants.CommandTimeout);
                    npgsqlOptions.EnableRetryOnFailure(
                        DbContextConstants.MaxRetryCount,
                        DbContextConstants.MaxRetryDelay,
                        []);
                })
            .UseExceptionProcessor();
    }

    /// <summary>
    ///     Builds and returns a configured <see cref="NpgsqlDataSource" /> with PostGIS support.
    ///     Parameter logging is enabled in Development, Local, and Test environments.
    /// </summary>
    /// <param name="env">The host environment determining whether parameter logging is activated.</param>
    /// <param name="connectionOptions">The database options providing the HTML-decoded connection string.</param>
    /// <returns>A ready-to-use <see cref="NpgsqlDataSource" /> that must be disposed with the application.</returns>
    [MustDisposeResource]
    private static NpgsqlDataSource GetNpgsqlDataSource(IHostEnvironment env,
        AppDbOptions connectionOptions)
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionOptions.DecodedConnectionString);
        if (env.EnvironmentName is Envs.Development or Envs.Local or Envs.Test)
            dataSourceBuilder.EnableParameterLogging();
        dataSourceBuilder.UseNetTopologySuite();

        return dataSourceBuilder.Build();
    }
}