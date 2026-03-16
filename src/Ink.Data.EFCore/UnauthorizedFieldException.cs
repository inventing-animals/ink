using System;

namespace Ink.Data.EFCore;

/// <summary>
/// Thrown when a <see cref="Queries.DataGridQuery"/> references a field that is not
/// registered in the <see cref="EFCoreQueryTranslator{T}"/>.
/// This acts as a security boundary - unregistered fields cannot be filtered, sorted,
/// or projected regardless of what the client sends.
/// </summary>
public sealed class UnauthorizedFieldException : Exception
{
    /// <summary>Gets the field name that was rejected.</summary>
    public string Field { get; }

    /// <summary>Initializes a new instance of <see cref="UnauthorizedFieldException"/>.</summary>
    /// <param name="field">The rejected field name.</param>
    public UnauthorizedFieldException(string field)
        : base($"Field '{field}' is not registered in the translator and cannot be queried.")
    {
        Field = field;
    }
}
