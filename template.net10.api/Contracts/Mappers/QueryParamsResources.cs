using template.net10.api.Domain.DTOs;

namespace template.net10.api.Contracts;

public sealed partial record QueryGetUserParamsResource
{
    /// <summary>
    ///     Implicitly converts a <see cref="QueryGetUserParamsResource"/> to a <see cref="QueryGetUserParamsDto"/>.
    /// </summary>
    public static implicit operator QueryGetUserParamsDto(QueryGetUserParamsResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new QueryGetUserParamsDto
        {
            Key = resource.Key
        };
    }

    /// <summary>
    ///     Converts the specified <see cref="QueryGetUserParamsResource"/> to a <see cref="QueryGetUserParamsDto"/>.
    /// </summary>
    public static QueryGetUserParamsDto ToQueryGetUserParamsDto(
        QueryGetUserParamsResource resource)
    {
        return resource;
    }
}

public sealed partial record QueryLoginUserParamsResource
{
    /// <summary>
    ///     Implicitly converts a <see cref="QueryLoginUserParamsResource"/> to a <see cref="QueryLoginUserParamsDto"/>.
    /// </summary>
    public static implicit operator QueryLoginUserParamsDto(QueryLoginUserParamsResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new QueryLoginUserParamsDto
        {
            Email = resource.Email,
            Password = resource.Password
        };
    }

    /// <summary>
    ///     Converts the specified <see cref="QueryLoginUserParamsResource"/> to a <see cref="QueryLoginUserParamsDto"/>.
    /// </summary>
    public static QueryLoginUserParamsDto ToQueryLoginUserParamsDto(
        QueryLoginUserParamsResource resource)
    {
        return resource;
    }
}