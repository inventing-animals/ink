namespace Ink.Data.Changes;

/// <summary>
/// Describes a single change to a data item, typically received from a SignalR hub
/// or other real-time channel.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
/// <param name="Type">The kind of change.</param>
/// <param name="Item">
/// The affected item. <c>null</c> when <see cref="ChangeType"/> is <see cref="ChangeType.Removed"/>
/// or <see cref="ChangeType.Invalidated"/>.
/// </param>
/// <param name="Key">
/// The primary key of the affected item. Always present, even when <paramref name="Item"/> is <c>null</c>.
/// </param>
public sealed record DataChange<T>(ChangeType Type, T? Item, object? Key);
