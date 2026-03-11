using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using template.net10.api.Persistence.Models;

namespace template.net10.api.Persistence.Context.Configuration;

/// <summary>
///     EF Core fluent configuration for the <see cref="User"/> entity.
///     Defines the primary key, server-side property defaults, and relationships with
///     <see cref="Role"/>, <see cref="Claim"/> and the self-referential <c>InsertUser</c>/<c>UpdateUser</c> links.
/// </summary>
internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    /// <summary>
    ///     Applies the full EF Core configuration for the <see cref="User"/> entity.
    /// </summary>
    /// <param name="builder">The entity type builder provided by EF Core.</param>
    public void Configure(EntityTypeBuilder<User> builder)
    {
        ConfigurePrimaryKeys(builder);
        ConfigureProperties(builder);
        ConfigureInsertUserRelationship(builder);
        ConfigureRoleRelationship(builder);
        ConfigureUpdateUserRelationship(builder);
        ConfigureClaimRelationship(builder);
    }

    /// <summary>
    ///     Configures the primary key for <see cref="User"/> with the constraint name <c>user_pkey</c>.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    private static void ConfigurePrimaryKeys(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(static e => e.Id).HasName("user_pkey");
    }

    /// <summary>
    ///     Configures server-side SQL default values for <c>InsertDatetime</c>, <c>UpdateDatetime</c>
    ///     (<c>now() AT TIME ZONE 'UTC'</c>) and <c>Uuid</c> (<c>gen_random_uuid()</c>).
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    private static void ConfigureProperties(EntityTypeBuilder<User> builder)
    {
        builder.Property(static e => e.InsertDatetime).HasDefaultValueSql("(now() AT TIME ZONE 'UTC'::text)");
        builder.Property(static e => e.UpdateDatetime).HasDefaultValueSql("(now() AT TIME ZONE 'UTC'::text)");
        builder.Property(static e => e.Uuid).HasDefaultValueSql("gen_random_uuid()");
    }

    /// <summary>
    ///     Configures the self-referential relationship for <see cref="User.InsertUser"/>
    ///     (the user who created the record) with <see cref="DeleteBehavior.Restrict"/>.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    private static void ConfigureInsertUserRelationship(EntityTypeBuilder<User> builder)
    {
        builder.HasOne(static d => d.InsertUser).WithMany(static p => p.InverseInsertUser)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("user_insert_user_id_fkey");
    }

    /// <summary>
    ///     Configures the many-to-one relationship between <see cref="User"/> and <see cref="Role"/>
    ///     with <see cref="DeleteBehavior.Restrict"/> and the constraint <c>user_role_id_fkey</c>.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    private static void ConfigureRoleRelationship(EntityTypeBuilder<User> builder)
    {
        builder.HasOne(static d => d.Role).WithMany(static p => p.Users)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("user_role_id_fkey");
    }

    /// <summary>
    ///     Configures the self-referential relationship for <see cref="User.UpdateUser"/>
    ///     (the user who last updated the record) with <see cref="DeleteBehavior.Restrict"/>.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    private static void ConfigureUpdateUserRelationship(EntityTypeBuilder<User> builder)
    {
        builder.HasOne(static d => d.UpdateUser).WithMany(static p => p.InverseUpdateUser)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("user_update_user_id_fkey");
    }

    /// <summary>
    ///     Configures the many-to-many relationship between <see cref="User"/> and <see cref="Claim"/>
    ///     via the <c>claim_user</c> join table in the <c>identity</c> schema with <see cref="DeleteBehavior.Restrict"/>.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    private static void ConfigureClaimRelationship(EntityTypeBuilder<User> builder)
    {
        builder.HasMany(static d => d.Claims).WithMany(static p => p.Users)
            .UsingEntity<Dictionary<string, object>>(
                "ClaimUser", static r => r.HasOne<Claim>().WithMany()
                    .HasForeignKey("ClaimId")
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("claim_user_claim_id_fkey"), static l => l.HasOne<User>().WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("claim_user_user_id_fkey"), static j =>
                {
                    j.HasKey("UserId", "ClaimId").HasName("claim_user_pkey");
                    j.ToTable("claim_user", "identity");
                    j.IndexerProperty<short>("UserId").HasColumnName("user_id");
                    j.IndexerProperty<short>("ClaimId").HasColumnName("claim_id");
                });
    }
}