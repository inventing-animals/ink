using System;

namespace Ink.DataGrid.Filtering;

/// <summary>
/// Returns a default <see cref="IColumnFilterEditor"/> for a given value type.
/// Called automatically by <see cref="Ink.DataGrid.Columns.DataGridColumn{TItem,TValue}"/>.
/// </summary>
public static class DefaultFilterEditor
{
    /// <summary>
    /// Returns the default filter editor for <typeparamref name="TValue"/>, or <c>null</c>
    /// if no built-in editor exists for the type.
    /// </summary>
    public static IColumnFilterEditor? For<TValue>()
    {
        var t = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);

        if (t == typeof(string))                                             return new TextFilterEditor();
        if (t == typeof(bool))                                               return new BoolFilterEditor();
        if (t.IsEnum)                                                        return new SelectionFilterEditor(t);
        if (t == typeof(DateTime) || t == typeof(DateTimeOffset)
                                  || t == typeof(DateOnly))                  return new DateRangeFilterEditor();
        if (t == typeof(int)   || t == typeof(long)  || t == typeof(short)
         || t == typeof(float) || t == typeof(double)|| t == typeof(decimal)) return new NumericRangeFilterEditor();

        return null;
    }
}
