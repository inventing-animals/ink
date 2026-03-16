namespace Ink.Data.Changes;

/// <summary>The type of change that occurred to a data item.</summary>
public enum ChangeType
{
    /// <summary>A new item was added.</summary>
    Added,

    /// <summary>An existing item was updated.</summary>
    Updated,

    /// <summary>An item was removed.</summary>
    Removed,

    /// <summary>
    /// The data set has changed in a way that cannot be expressed as individual row changes.
    /// Sources should re-query entirely when receiving this type.
    /// </summary>
    Invalidated,
}
