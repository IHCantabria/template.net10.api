using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace template.net10.api.Persistence.Context;

internal partial class AppDbContext
{
    /// <summary>
    ///     Extension point invoked by EF Core during model creation to apply additional entity
    ///     configurations defined in partial class extensions. This default implementation performs
    ///     no configuration and is intended to be overridden in derived partial classes.
    /// </summary>
    /// <param name="modelBuilder">The builder used to construct the EF Core model for this context.</param>
    [SuppressMessage("Roslynator", "RCS1163:Unused parameter",
        Justification = "Template method for partial extensions")]
    [SuppressMessage("ReSharper", "UnusedParameter.Local", Justification = "Template method for partial extensions")]
    [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local",
        Justification = "Template method for partial extensions")]
    [SuppressMessage("Performance", "CA1822:Mark members as static",
        Justification = "Template method for partial extensions")]
    private void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        // No business model configuration by default. This method is a template for partial extensions in the future.
    }
}