using JetBrains.Annotations;

namespace template.net10.api.Core.Attributes;

/// <summary>
///     ADD DOCUMENTATION
/// </summary>
[MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]
[AttributeUsage(AttributeTargets.Interface)]
internal sealed class PublicApiContractAttribute : Attribute;