using template.net10.api.Domain.DTOs;

namespace template.net10.api.Contracts;

public sealed partial record CommandCreateUserParamsResource
{
    /// <summary>
    ///     Implicitly converts a <see cref="CommandCreateUserParamsResource"/> to a <see cref="CommandCreateUserParamsDto"/>.
    /// </summary>
    public static implicit operator CommandCreateUserParamsDto(CommandCreateUserParamsResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new CommandCreateUserParamsDto
        {
            Username = resource.Username,
            Email = resource.Email,
            IsDisabled = resource.IsDisabled,
            FirstName = resource.FirstName,
            LastName = resource.LastName,
            Password = resource.Password,
            ConfirmPassword = resource.ConfirmPassword,
            RoleId = resource.RoleId,
            Identity = new IdentityDto()
        };
    }

    /// <summary>
    ///     Converts the specified <see cref="CommandCreateUserParamsResource"/> to a <see cref="CommandCreateUserParamsDto"/>.
    /// </summary>
    public static CommandCreateUserParamsDto ToCommandCreateUserParamsDto(
        CommandCreateUserParamsResource resource)
    {
        return resource;
    }
}

public sealed partial record CommandUpdateUserParamsResource
{
    /// <summary>
    ///     Implicitly converts a <see cref="CommandUpdateUserParamsResource"/> to a <see cref="CommandUpdateUserParamsDto"/>.
    /// </summary>
    public static implicit operator CommandUpdateUserParamsDto(CommandUpdateUserParamsResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new CommandUpdateUserParamsDto
        {
            Key = resource.Key,
            Username = resource.Body.Username,
            Email = resource.Body.Email,
            IsDisabled = resource.Body.IsDisabled,
            RoleId = resource.Body.RoleId,
            FirstName = resource.Body.FirstName,
            LastName = resource.Body.LastName,
            Identity = new IdentityDto()
        };
    }

    /// <summary>
    ///     Converts the specified <see cref="CommandUpdateUserParamsResource"/> to a <see cref="CommandUpdateUserParamsDto"/>.
    /// </summary>
    public static CommandUpdateUserParamsDto ToCommandUpdateUserParamsDto(
        CommandUpdateUserParamsResource resource)
    {
        return resource;
    }
}

public sealed partial record CommandDisableUserParamsResource
{
    /// <summary>
    ///     Implicitly converts a <see cref="CommandDisableUserParamsResource"/> to a <see cref="CommandDisableUserParamsDto"/>.
    /// </summary>
    public static implicit operator CommandDisableUserParamsDto(CommandDisableUserParamsResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new CommandDisableUserParamsDto
        {
            Key = resource.Key,
            Identity = new IdentityDto()
        };
    }

    /// <summary>
    ///     Converts the specified <see cref="CommandDisableUserParamsResource"/> to a <see cref="CommandDisableUserParamsDto"/>.
    /// </summary>
    public static CommandDisableUserParamsDto ToCommandDisableUserParamsDto(
        CommandDisableUserParamsResource resource)
    {
        return resource;
    }
}

public sealed partial record CommandEnableUserParamsResource
{
    /// <summary>
    ///     Implicitly converts a <see cref="CommandEnableUserParamsResource"/> to a <see cref="CommandEnableUserParamsDto"/>.
    /// </summary>
    public static implicit operator CommandEnableUserParamsDto(CommandEnableUserParamsResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new CommandEnableUserParamsDto
        {
            Key = resource.Key,
            Identity = new IdentityDto()
        };
    }

    /// <summary>
    ///     Converts the specified <see cref="CommandEnableUserParamsResource"/> to a <see cref="CommandEnableUserParamsDto"/>.
    /// </summary>
    public static CommandEnableUserParamsDto ToCommandEnableUserParamsDto(
        CommandEnableUserParamsResource resource)
    {
        return resource;
    }
}

public sealed partial record CommandDeleteUserParamsResource
{
    /// <summary>
    ///     Implicitly converts a <see cref="CommandDeleteUserParamsResource"/> to a <see cref="CommandDeleteUserParamsDto"/>.
    /// </summary>
    public static implicit operator CommandDeleteUserParamsDto(CommandDeleteUserParamsResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new CommandDeleteUserParamsDto
        {
            Key = resource.Key,
            Identity = new IdentityDto()
        };
    }

    /// <summary>
    ///     Converts the specified <see cref="CommandDeleteUserParamsResource"/> to a <see cref="CommandDeleteUserParamsDto"/>.
    /// </summary>
    public static CommandDeleteUserParamsDto ToCommandDeleteUserParamsDto(
        CommandDeleteUserParamsResource resource)
    {
        return resource;
    }
}

public sealed partial record CommandResetUserPasswordParamsResource
{
    /// <summary>
    ///     Implicitly converts a <see cref="CommandResetUserPasswordParamsResource"/> to a <see cref="CommandResetUserPasswordParamsDto"/>.
    /// </summary>
    public static implicit operator CommandResetUserPasswordParamsDto(CommandResetUserPasswordParamsResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new CommandResetUserPasswordParamsDto
        {
            Key = resource.Key,
            Password = resource.Body.Password,
            ConfirmPassword = resource.Body.ConfirmPassword,
            Identity = new IdentityDto()
        };
    }

    /// <summary>
    ///     Converts the specified <see cref="CommandResetUserPasswordParamsResource"/> to a <see cref="CommandResetUserPasswordParamsDto"/>.
    /// </summary>
    public static CommandResetUserPasswordParamsDto ToCommandResetUserPasswordParamsDto(
        CommandResetUserPasswordParamsResource resource)
    {
        return resource;
    }
}