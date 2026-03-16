namespace Ink.Data.Columns;

/// <summary>
/// Non-generic interface for a column definition, used by grids and other
/// data-aware controls that don't know the concrete item type at compile time.
/// </summary>
public interface IColumn
{
    /// <summary>Gets the display name shown in the column header.</summary>
    string Header { get; }

    /// <summary>Gets the optional description shown as a tooltip on the header.</summary>
    string? Description { get; }

    /// <summary>Gets the camelCase field name used in queries and wire protocols.</summary>
    string FieldName { get; }

    /// <summary>Returns the column value for <paramref name="item"/> as an untyped object.</summary>
    object? GetValue(object item);
}
