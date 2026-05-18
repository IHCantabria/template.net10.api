using System.Diagnostics.CodeAnalysis;
using Serilog;

namespace template.net10.api.Logger.Business;

/// <summary>
///     Provides business-level log minimum level overrides for third-party libraries and
///     application-specific namespaces. These overrides complement or supersede the core
///     infrastructure overrides defined in <c>LoggerConfigurationExtensions.ConfigureMinLevels</c>.
/// </summary>
internal static class BusinessLoggerConfigurationExtensions
{
    extension(LoggerConfiguration lc)
    {
        /// <summary>
        ///     Configures minimum log level overrides for business and third-party dependencies
        ///     such as YARP reverse proxy. Overrides defined here take precedence over broader
        ///     namespace overrides because Serilog applies the most specific match.
        /// </summary>
        /// <returns>The <see cref="LoggerConfiguration" /> with business-level overrides applied.</returns>
        [SuppressMessage(
            "ReSharper",
            "ExceptionNotDocumentedOptional",
            Justification =
                "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
        internal LoggerConfiguration ConfigureBusinessMinLevels()
        {
            return lc;

            // add here business log level overrides as needed
        }
    }
}