using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using template.net10.api.Persistence.Models;
using template.net10.api.Settings.Options;
using Version = template.net10.api.Persistence.Models.Version;

namespace template.net10.api.Persistence.Context;

/// <summary>
///     Main Entity Framework Core database context for the application.
///     Exposes <see cref="DbSet{TEntity}" /> collections for all domain entities and applies all
///     assembly-level entity configurations. Supports dynamic schema assignment via <see cref="AppDbOptions" />.
/// </summary>
[MustDisposeResource]
internal partial class AppDbContext(DbContextOptions<AppDbContext> options, IOptions<AppDbOptions> config)
    : DbContext(options)
{
    /// <summary>
    ///     Database configuration options (e.g. schema override) resolved from <see cref="AppDbOptions" />.
    /// </summary>
    private readonly AppDbOptions _config = config.Value ?? throw new ArgumentNullException(nameof(config));

    /// <summary>
    ///     Gets the <see cref="DbSet{TEntity}" /> for <see cref="Claim" /> records (permission entries).
    /// </summary>
    public virtual DbSet<Claim> Claims { get; set; }

    /// <summary>
    ///     Gets the <see cref="DbSet{TEntity}" /> for <see cref="CurrentVersion" /> records
    ///     (tracks which version is currently active).
    /// </summary>
    public virtual DbSet<CurrentVersion> CurrentVersions { get; set; }

    /// <summary>
    ///     Gets the <see cref="DbSet{TEntity}" /> for <see cref="Role" /> records.
    /// </summary>
    public virtual DbSet<Role> Roles { get; set; }

    /// <summary>
    ///     Gets the <see cref="DbSet{TEntity}" /> for <see cref="User" /> records.
    /// </summary>
    public virtual DbSet<User> Users { get; set; }

    /// <summary>
    ///     Gets the <see cref="DbSet{TEntity}" /> for <see cref="Version" /> records (application version history).
    /// </summary>
    public virtual DbSet<Version> Versions { get; set; }

    /// <summary>
    ///     Configures the entity model by applying all <see cref="IEntityTypeConfiguration{T}" /> implementations
    ///     found in the assembly. If <see cref="AppDbOptions.Schema" /> is set, overrides the schema annotation
    ///     on every entity type.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="modelBuilder" /> is <see langword="null" />.</exception>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        OnModelCreatingPartial(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Program).Assembly);

        if (_config.Schema is null) return;
        //Must be Serial
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.RemoveAnnotation("Relational:Schema");

            entity.AddAnnotation("Relational:Schema", _config.Schema);
        }
    }
}