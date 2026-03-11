using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net.Http.Headers;
using System.Numerics;
using System.Runtime.InteropServices;
using LanguageExt;
using Microsoft.IdentityModel.Protocols.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.Destructurers;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using Serilog.Formatting.Compact;
using Serilog.Sinks.OpenTelemetry;
using template.net10.api.Business;
using template.net10.api.Core.Logger.Enrichers;
using template.net10.api.Core.OpenTelemetry.Options;
using template.net10.api.Settings.Options;
using Path = System.IO.Path;

namespace template.net10.api.Core.Logger.Extensions;

/// <summary>
///     Provides extension methods for <see cref="LoggerConfiguration" /> to configure enrichment, minimum levels, and
///     sinks.
/// </summary>
internal static class LoggerConfigurationExtensions
{
    /// <summary>
    ///     Checks whether the OpenTelemetry log endpoint is reachable by sending a minimal HTTP request.
    /// </summary>
    /// <param name="config">The OpenTelemetry options containing the log endpoint configuration.</param>
    /// <returns>A <see cref="LanguageExt.Try{T}" /> indicating whether the endpoint responded successfully.</returns>
    private static Try<bool> IsLogOpenTelemetryAvailable(OpenTelemetryOptions config)
    {
        return () =>
        {
            using var client = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Post, config.LogEndpointUrl);

            if (config.UseLogHeaderApiKey() && config.LogEndpointApiKeyHeader != null)
                request.Headers.Add(config.LogEndpointApiKeyHeader, config.LogEndpointApiKeyValue);

            // Send a minimal valid OpenTelemetry log entry
            request.Content = new ByteArrayContent([]);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-protobuf");

            // Check if client responds
            using var response = client.Send(request, HttpCompletionOption.ResponseHeadersRead);
            return response.IsSuccessStatusCode;
        };
    }

    extension(LoggerConfiguration lc)
    {
        /// <summary>
        ///     Configures log enrichment with activity tracing, request identifiers, thread info, client IP, correlation ID, and
        ///     exception details.
        /// </summary>
        /// <returns>The enriched <see cref="LoggerConfiguration" />.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Given depth must be positive.</exception>
        [SuppressMessage(
            "ReSharper",
            "ExceptionNotDocumentedOptional",
            Justification =
                "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
        internal LoggerConfiguration EnrichLog()
        {
            var destructures = new List<IExceptionDestructurer> { new DbUpdateExceptionDestructurer() };
            destructures.AddRange(DestructuringOptionsBuilder.DefaultDestructurers);

            return lc.Enrich.FromLogContext()
                .Enrich.With<ActivityEnricher>()
                .Enrich.With<RequestIdentifierEnricher>()
                .Enrich.With<ThreadIdEnricher>()
                .Enrich.With<ThreadNameEnricher>()
                .Enrich.With<ClientIpEnricher>()
                .Enrich.With(new CorrelationIdEnricher("x-correlation-id", false))
                .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                    .WithDestructurers(destructures)
                    .WithDestructuringDepth(1)
                    .WithoutReflectionBasedDestructurer());
        }

        /// <summary>
        ///     Configures minimum log level overrides for Microsoft, System, Npgsql, and ASP.NET Core namespaces.
        /// </summary>
        /// <returns>The <see cref="LoggerConfiguration" /> with minimum level overrides applied.</returns>
        [SuppressMessage(
            "ReSharper",
            "ExceptionNotDocumentedOptional",
            Justification =
                "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
        internal LoggerConfiguration ConfigureMinLevels()
        {
            return lc.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Information)
                .MinimumLevel.Override("Program", LogEventLevel.Information)
                .MinimumLevel.Override("Npgsql", LogEventLevel.Information)
                .MinimumLevel.Override(BusinessConstants.ApiName, LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.DataProtection.KeyManagement.XmlKeyManager",
                    LogEventLevel.Error)
                .MinimumLevel.Override("Microsoft.AspNetCore.DataProtection.Repositories.EphemeralXmlRepository",
                    LogEventLevel.Error);
        }

        /// <summary>
        ///     Configures log sinks based on OpenTelemetry settings, falling back to local file sink if OpenTelemetry is inactive.
        /// </summary>
        /// <param name="builderConfiguration">The application configuration manager.</param>
        /// <param name="envName">The current environment name.</param>
        /// <param name="version">The application version.</param>
        /// <returns>The <see cref="LoggerConfiguration" /> with sinks configured.</returns>
        /// <exception cref="InvalidConfigurationException">Thrown when the OpenTelemetry log endpoint is unreachable or the OpenTelemetry/API configuration section in the appsettings file is missing or invalid.</exception>
        internal LoggerConfiguration ConfigureSinks(ConfigurationManager builderConfiguration, string envName,
            string version)
        {
            var openTelemetryOptions = builderConfiguration.GetSection(OpenTelemetryOptions.OpenTelemetry)
                .Get<OpenTelemetryOptions>();

            if (openTelemetryOptions?.IsLogActive != true)
                return lc.ConfigureSinkLocal();

            OptionsValidator.ValidateOpenTelemetryOptions(openTelemetryOptions);
            var useOpenTelemetry = IsLogOpenTelemetryAvailable(openTelemetryOptions).Try();
            if (useOpenTelemetry.IsFaulted)
                throw new InvalidConfigurationException(
                    "The OpenTelemetry Log configuration in the appsettings file is incorrect or the endpoint for the logs is down. There was a problem trying to connecte to the OpenTelemetry log endpoint");

            var apiOptionsConfig = builderConfiguration.GetSection(ApiOptions.Api).Get<ApiOptions>();
            OptionsValidator.ValidateApiOptions(apiOptionsConfig);
            if (apiOptionsConfig is null)
                throw new InvalidConfigurationException(
                    "The Api configuration in the appsettings file is incorrect.");

            var config = new OpenTelemetryConfig
            {
                OpenTelemetryOptions = openTelemetryOptions,
                ApiOptions = apiOptionsConfig,
                EnvName = envName,
                Version = version
            };
            return lc.ConfigureSinkTelemetry(config);
        }

        /// <summary>
        ///     Configures an OpenTelemetry sink with service resource attributes, protocol, and endpoint settings.
        /// </summary>
        /// <param name="config">The OpenTelemetry configuration containing endpoint, API, environment, and version details.</param>
        /// <returns>The <see cref="LoggerConfiguration" /> with the OpenTelemetry sink configured.</returns>
        private LoggerConfiguration ConfigureSinkTelemetry(OpenTelemetryConfig config)
        {
            return lc.WriteTo.Async(s => s.OpenTelemetry(x =>
            {
                x.LogsEndpoint =
                    config.OpenTelemetryOptions.LogEndpointUrl
                        .ToString(); //for loki use http://localhost:3100/otlp // for seq use http://localhost:5341/ingest/otlp/v1/logs
                x.Protocol = OtlpProtocol.HttpProtobuf;
                x.HttpMessageHandler = new SocketsHttpHandler { ActivityHeadersPropagator = null };
                if (config.OpenTelemetryOptions.UseLogHeaderApiKey() &&
                    config.OpenTelemetryOptions.LogEndpointApiKeyHeader != null &&
                    config.OpenTelemetryOptions.LogEndpointApiKeyValue != null)
                    x.Headers = new Dictionary<string, string>
                    {
                        [config.OpenTelemetryOptions.LogEndpointApiKeyHeader] =
                            config.OpenTelemetryOptions.LogEndpointApiKeyValue
                    };
                x.ResourceAttributes = new Dictionary<string, object>
                {
                    ["service.name"] = config.OpenTelemetryOptions.ServiceName,
                    ["service.version"] = config.Version,
                    ["service.instance.id"] = CoreConstants.GuidInstance.ToString(),
                    ["service.thread.id"] = Environment.CurrentManagedThreadId,
                    ["service.thread.name"] = Thread.CurrentThread.Name ?? string.Empty,
                    ["server.address"] = config.ApiOptions.Address,
                    ["service.environment"] = config.EnvName,
                    ["host.name"] = Environment.MachineName,
                    ["host.ip"] = HostInfo.GetHostIp(),
                    ["os.description"] = RuntimeInformation.OSDescription,
                    ["os.architecture"] = RuntimeInformation.OSArchitecture.ToString(),
                    ["process.runtime.name"] = ".NET 10",
                    ["process.runtime.version"] = Environment.Version.ToString(),
                    ["process.user.name"] = Environment.UserName,
                    ["process.pid"] = Environment.ProcessId,
                    ["process.name"] = Process.GetCurrentProcess().ProcessName
                };
                x.FormatProvider = CultureInfo.InvariantCulture;
            }));
        }

        /// <summary>
        ///     Configures a local file sink that writes logs in compact JSON format with daily rolling.
        /// </summary>
        /// <returns>The <see cref="LoggerConfiguration" /> with a local file sink configured.</returns>
        [SuppressMessage(
            "ReSharper",
            "ExceptionNotDocumentedOptional",
            Justification =
                "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
        internal LoggerConfiguration ConfigureSinkLocal()
        {
            var logPath = Path.Combine(AppContext.BaseDirectory, "logs", "log.txt");
            return lc.WriteTo.Async(c => c.File(
                new CompactJsonFormatter(),
                logPath,
                buffered: true,
                rollingInterval: RollingInterval.Day,
                rollOnFileSizeLimit: true));
        }
    }
}

/// <summary>
///     Configuration record that aggregates OpenTelemetry, API options, environment name, and version for sink setup.
/// </summary>
internal sealed record OpenTelemetryConfig : IEqualityOperators<OpenTelemetryConfig, OpenTelemetryConfig, bool>
{
    /// <summary>
    ///     Gets the current deployment environment name (e.g., dev, test, prod).
    /// </summary>
    public required string EnvName { get; init; }

    /// <summary>
    ///     Gets the application version string.
    /// </summary>
    public required string Version { get; init; }

    /// <summary>
    ///     Gets the OpenTelemetry endpoint and authentication options.
    /// </summary>
    public required OpenTelemetryOptions OpenTelemetryOptions { get; init; }

    /// <summary>
    ///     Gets the API configuration options including service address.
    /// </summary>
    public required ApiOptions ApiOptions { get; init; }
}