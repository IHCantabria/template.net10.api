using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using template.net10.api.Settings.Interfaces;
using ZLinq;
using ZLinq.Linq;

namespace template.net10.api.Settings.Extensions;

/// <summary>
///     Extension methods for <see cref="WebApplication"/> to auto-discover and run all
///     <see cref="IPipelineConfigurator"/> implementations and to configure localization middleware.
/// </summary>
internal static class WebApplicationExtensions
{
    /// <summary>
    ///     Reflects over the assembly to locate every non-abstract type that implements
    ///     <see cref="IPipelineConfigurator"/> and returns an activated instance of each.
    /// </summary>
    /// <returns>An enumerable of <see cref="IPipelineConfigurator"/> instances ready to be ordered and invoked.</returns>
    private static
        ValueEnumerable<Cast<ArrayWhereSelect<Type, object?>, object?, IPipelineConfigurator>, IPipelineConfigurator>
        GetPipelineConfigurators()
    {
        //Get all Types in the assembly that implement IConfigurator, create a instance of the type and order it by LoadOrder.
        var exportedTypes = typeof(Program).Assembly.GetTypes().Where(static x =>
            typeof(IPipelineConfigurator).IsAssignableFrom(x) && x is { IsInterface: false, IsAbstract: false });
        return exportedTypes.Select(Activator.CreateInstance).Cast<IPipelineConfigurator>();
    }

    extension(WebApplication app)
    {
        /// <summary>
        ///     Registers the request localization middleware with <see cref="CultureInfo.InvariantCulture"/>
        ///     as the default request culture, ensuring consistent culture-neutral formatting
        ///     for non-localizable operations.
        /// </summary>
        [SuppressMessage(
            "ReSharper",
            "ExceptionNotDocumentedOptional",
            Justification =
                "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
        internal void ConfigureLocalizationMiddleware()
        {
            // Define the supported cultures
            // Configure the Request Localization options
            var requestLocalizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(CultureInfo.InvariantCulture)
            };
            // Add the Request Localization middleware
            app.UseRequestLocalization(requestLocalizationOptions);
        }
    }

    extension(WebApplication app)
    {
        /// <summary>
        ///     Discovers all <see cref="IPipelineConfigurator"/> implementations in the executing assembly
        ///     and invokes them in ascending <see cref="IPipelineConfigurator.LoadOrder"/> order.
        ///     Execution is intentionally serial to respect configurator dependencies.
        /// </summary>
        internal async Task ConfigurePipelinesInAssemblyAsync()
        {
            var pipelines = GetPipelineConfigurators();
            //MUST BE Serial, keeping order of the LoadOrder
            foreach (var t in pipelines.OrderBy(static o => o.LoadOrder).ToList())
                await t.ConfigurePipelineAsync(app).ConfigureAwait(false);
        }
    }
}