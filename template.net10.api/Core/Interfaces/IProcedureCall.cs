using System.Diagnostics.CodeAnalysis;

namespace template.net10.api.Core.Interfaces;

/// <summary>
///     Defines the contract for a stored procedure call, including its name and parameters.
/// </summary>
[SuppressMessage("Design",
    "CA1515:Consider making public types internal",
    Justification =
        "The interface is part of the public API contract and must remain publicly accessible.")]
internal interface IProcedureCall
{
    /// <summary>
    ///     Gets the name of the stored procedure to execute.
    /// </summary>
    string ProcedureName { get; }

    /// <summary>
    ///     Gets the collection of parameters to pass to the stored procedure.
    /// </summary>
    IEnumerable<object> Parameters { get; }
}