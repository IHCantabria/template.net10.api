using template.net10.api.Settings.Interfaces;
using ZLinq;
using ZLinq.Linq;

namespace template.net10.api.Settings.Extensions;

/// <summary>
///     Extension methods for <see cref="WebApplicationBuilder"/> to auto-discover and run
///     all <see cref="IServiceInstaller"/> implementations registered in the assembly.
/// </summary>
internal static class WebApplicationBuilderExtensions
{
    /// <summary>
    ///     Reflects over the assembly to locate every non-abstract type that implements
    ///     <see cref="IServiceInstaller"/> and returns an activated instance of each.
    /// </summary>
    /// <returns>An enumerable of <see cref="IServiceInstaller"/> instances ready to be ordered and invoked.</returns>
    private static ValueEnumerable<Cast<ArrayWhereSelect<Type, object?>, object?, IServiceInstaller>, IServiceInstaller>
        GetServiceInstallers()
    {
        //Get all Types in the assembly that implement IInstaller, create a instance of the type and order it by LoadOrder.
        var exportedTypes = typeof(Program).Assembly.GetTypes().Where(static x =>
            typeof(IServiceInstaller).IsAssignableFrom(x) && x is { IsInterface: false, IsAbstract: false });
        return exportedTypes.Select(Activator.CreateInstance).Cast<IServiceInstaller>();
    }

    extension(WebApplicationBuilder builder)
    {
        /// <summary>
        ///     Discovers all <see cref="IServiceInstaller"/> implementations in the executing assembly
        ///     and invokes them in ascending <see cref="IServiceInstaller.LoadOrder"/> order.
        ///     Execution is intentionally serial to respect installer dependencies.
        /// </summary>
        internal async Task InstallServicesInAssemblyAsync()
        {
            var services = GetServiceInstallers();
            //Call the InstallServices for all IInstaller implementations.
            //MUST BE Serial, keeping order of the LoadOrder
            foreach (var t in services.OrderBy(static c => c.LoadOrder).ToList())
                await t.InstallServiceAsync(builder).ConfigureAwait(false);
        }
    }
}