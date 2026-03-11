using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Serilog.Core;
using template.net10.api.Core.Logger.Sinks;
using ILogger = Serilog.ILogger;

namespace template.net10.api.Core.Logger.Extensions;

/// <summary>
///     Provides extension methods for Serilog <see cref="ILogger" /> to inspect internal sink configuration.
/// </summary>
internal static class LoggerExtensions
{
    /// <summary>
    ///     Determines whether the specified sink is or wraps a <see cref="MemorySink" /> by inspecting internal fields via
    ///     reflection.
    /// </summary>
    /// <param name="sink">The log event sink to inspect.</param>
    /// <returns>
    ///     <see langword="true" /> if the sink is or wraps a <see cref="MemorySink" />; otherwise,
    ///     <see langword="false" />.
    /// </returns>
    [SuppressMessage("Security",
        "S3011:Reflection should not be used to increase accessibility of classes, methods, or fields",
        Justification =
            "Reflection is required to access internal Serilog state because no public API is available to determine whether sinks are configured.")]
    private static bool IsOrWrapsMemorySink(ILogEventSink sink)
    {
        switch (sink)
        {
            case null:
                return false;
            case MemorySink:
                return true;
        }

        var wrappedSinkField = sink.GetType().GetField("_wrappedSink", BindingFlags.NonPublic | BindingFlags.Instance);
        if (wrappedSinkField == null) return false;

        return wrappedSinkField.GetValue(sink) is ILogEventSink inner
               && IsOrWrapsMemorySink(inner);
    }

    extension(ILogger? logger)
    {
        /// <summary>
        ///     Checks whether the current Serilog logger has any non-memory sinks configured, using reflection to access internal
        ///     pipeline state.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> if at least one non-<see cref="MemorySink" /> sink is configured; otherwise,
        ///     <see langword="false" />.
        /// </returns>
        /// <exception cref="NotSupportedException">
        ///     A field is marked literal, but the field does not have one of the accepted
        ///     literal types.
        /// </exception>
        /// <exception cref="FieldAccessException">
        ///     The caller does not have permission to access this field.
        ///     Note: In .NET for Windows Store apps or the Portable Class Library, catch the base class exception,
        ///     <see cref="MemberAccessException" />, instead.
        /// </exception>
        [SuppressMessage("Security",
            "S3011:Reflection should not be used to increase accessibility of classes, methods, or fields",
            Justification =
                "Reflection is required to access internal Serilog state because no public API is available to determine whether sinks are configured.")]
        [SuppressMessage(
            "ReSharper",
            "ExceptionNotDocumentedOptional",
            Justification =
                "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
        [SuppressMessage(
            "ReSharper",
            "ExceptionNotDocumented",
            Justification =
                "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
        internal bool CurrentLoggerHasSinks()
        {
            var innerLoggerField =
                logger?.GetType().GetField("_logger", BindingFlags.NonPublic | BindingFlags.Instance);
            if (innerLoggerField != null) logger = innerLoggerField.GetValue(logger) as Serilog.Core.Logger;

            var pipelineField =
                typeof(Serilog.Core.Logger).GetField("_sink", BindingFlags.NonPublic | BindingFlags.Instance);
            if (pipelineField == null) return false;

            var pipeline = pipelineField.GetValue(logger);
            if (pipeline == null) return false;

            var sinksField = pipeline.GetType().GetField("_sinks", BindingFlags.NonPublic | BindingFlags.Instance);
            if (sinksField == null) return false;

            var sinks = sinksField.GetValue(pipeline) as IEnumerable;
            return sinks?.Cast<ILogEventSink>().Any(static s => !IsOrWrapsMemorySink(s)) == true;
        }
    }
}