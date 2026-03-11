using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace template.net10.api.Settings.Attributes;

/// <summary>
///     Marks a controller action or class as visible in Swagger only in development-like environments
///     (local, test, dev). In production and pre-production the endpoint is hidden from the API documentation.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
internal sealed class DevSwaggerAttribute : Attribute;

/// <summary>
///     ASP.NET Core action model convention that hides endpoints decorated with <see cref="DevSwaggerAttribute"/>
///     from the Swagger UI when the application is not running in a development-like environment.
/// </summary>
internal sealed class ActionHidingConvention(string envName) : IActionModelConvention
{
    /// <summary>
    ///     <see langword="true"/> when the current environment is local, test, or dev; <see langword="false"/> otherwise.
    ///     Controls Swagger visibility for endpoints tagged with <see cref="DevSwaggerAttribute"/>.
    /// </summary>
    private readonly bool _isDevelopment = envName is Envs.Development or Envs.Local or Envs.Test;

    /// <summary>
    ///     Applies the convention to the given <paramref name="action"/>, toggling its Swagger visibility
    ///     based on whether it carries <see cref="DevSwaggerAttribute"/> and the current environment.
    /// </summary>
    /// <param name="action">The action model to evaluate.</param>
    public void Apply(ActionModel action)
    {
        action.ApiExplorer.IsVisible = ShouldDisplaySwagger(action);
    }

    /// <summary>
    ///     Returns <see langword="true"/> if the action should appear in Swagger:
    ///     either the environment is development-like, or the action has no <see cref="DevSwaggerAttribute"/>.
    /// </summary>
    /// <param name="action">The action or controller model to inspect.</param>
    /// <returns><see langword="true"/> to show the endpoint; <see langword="false"/> to hide it.</returns>
    private bool ShouldDisplaySwagger(ICommonModel action)
    {
        // Swagger is visible by default. Only hide if DevSwagger attribute is present and _isDevelopment is false.
        return _isDevelopment || !HasDevSwaggerAttribute(action);
    }

    /// <summary>
    ///     Returns <see langword="true"/> if the action's controller or the action itself carries <see cref="DevSwaggerAttribute"/>.
    /// </summary>
    /// <param name="action">The action or controller model to inspect.</param>
    /// <returns><see langword="true"/> if the dev-only attribute is present on either the controller or the action.</returns>
    private static bool HasDevSwaggerAttribute(ICommonModel action)
    {
        return ControllerHasDevSwaggerAttribute(action) || ActionHasDevSwaggerAttribute(action);
    }

    /// <summary>
    ///     Returns <see langword="true"/> if the controller that owns <paramref name="action"/> carries <see cref="DevSwaggerAttribute"/>.
    /// </summary>
    /// <param name="action">The action model whose declaring controller is inspected.</param>
    /// <returns><see langword="true"/> when the controller-level attribute is found.</returns>
    private static bool ControllerHasDevSwaggerAttribute(ICommonModel action)
    {
        var actionModel = (ActionModel)action;
        return actionModel.Controller.Attributes.Any(IsDevSwaggerAttribute);
    }

    /// <summary>
    ///     Returns <see langword="true"/> if <paramref name="action"/> itself carries <see cref="DevSwaggerAttribute"/>.
    /// </summary>
    /// <param name="action">The action or controller model to inspect.</param>
    /// <returns><see langword="true"/> when the action-level attribute is found.</returns>
    private static bool ActionHasDevSwaggerAttribute(ICommonModel action)
    {
        return action.Attributes.Any(IsDevSwaggerAttribute);
    }

    /// <summary>
    ///     Returns <see langword="true"/> when <paramref name="attribute"/> is an instance of
    ///     <see cref="DevSwaggerAttribute"/>, used as a predicate for LINQ attribute lookups.
    /// </summary>
    /// <param name="attribute">The attribute object to test.</param>
    /// <returns><see langword="true"/> if the attribute is a <see cref="DevSwaggerAttribute"/>; <see langword="false"/> otherwise.</returns>
    private static bool IsDevSwaggerAttribute(object attribute)
    {
        return attribute is DevSwaggerAttribute;
    }
}