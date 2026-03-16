using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Ink.Data.Columns;
using Ink.Data.EFCore.Internal;
using Ink.Data.Queries;
using Ink.Data.Results;
using Microsoft.EntityFrameworkCore;

namespace Ink.Data.EFCore;

/// <summary>
/// Translates a <see cref="DataQuery"/> into EF Core <see cref="IQueryable{T}"/> operations,
/// fully automatically handling filtering, sorting, and pagination.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
/// <example>
/// <code>
/// // Configure once (e.g. as a singleton or static field)
/// var translator = new EFCoreQueryTranslator&lt;User&gt;()
///     .Column(x => x.Name)
///     .Column(x => x.Email)
///     .Column(x => x.CreatedAt)
///     .Column(x => x.Status);
///
/// // In an API endpoint
/// app.MapPost("/api/users/query", (DataQuery query, AppDbContext db) =>
///     translator.ExecuteAsync(db.Users.Where(x => x.TenantId == tenantId), query));
/// </code>
/// </example>
public sealed class EFCoreQueryTranslator<T> where T : class
{
    private readonly Dictionary<string, Column<T>> _columns =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly Dictionary<FilterOp, Func<Expression, Type, IReadOnlyList<object?>, Expression>> _customOps = new();

    /// <summary>
    /// Registers a column using a typed selector expression.
    /// The field name is derived automatically in camelCase.
    /// </summary>
    /// <typeparam name="TValue">The column value type.</typeparam>
    /// <param name="selector">Member-access expression, e.g. <c>x => x.CreatedAt</c>.</param>
    /// <param name="ops">
    /// Override supported filter operations. When <c>null</c>, defaults are inferred
    /// from <typeparamref name="TValue"/> via <see cref="DefaultOps"/>.
    /// </param>
    /// <returns>The same translator for fluent chaining.</returns>
    public EFCoreQueryTranslator<T> Column<TValue>(
        Expression<Func<T, TValue>> selector,
        IReadOnlyList<FilterOp>? ops = null)
    {
        var col = new Column<T, TValue>(selector, ops);
        _columns[col.FieldName] = col;
        return this;
    }

    /// <summary>
    /// Registers a handler for a custom (or overridden built-in) filter operator.
    /// Custom handlers take precedence over built-in operator handling.
    /// </summary>
    /// <param name="op">The operator to handle.</param>
    /// <param name="handler">
    /// A factory that receives the member access expression, the member's CLR type,
    /// and the condition values, and returns an EF Core-compatible predicate expression.
    /// </param>
    /// <returns>The same translator for fluent chaining.</returns>
    /// <example>
    /// <code>
    /// translator.HandleOp(new FilterOp("fts"), (member, _, values) =>
    ///     Expression.Call(
    ///         typeof(NpgsqlDbFunctionsExtensions),
    ///         nameof(NpgsqlDbFunctionsExtensions.ToTsVector), ...));
    /// </code>
    /// </example>
    public EFCoreQueryTranslator<T> HandleOp(
        FilterOp op,
        Func<Expression, Type, IReadOnlyList<object?>, Expression> handler)
    {
        _customOps[op] = handler;
        return this;
    }

    /// <summary>
    /// Applies filter and sort from <paramref name="query"/> to <paramref name="source"/>
    /// and returns the resulting <see cref="IQueryable{T}"/>.
    /// Pagination is NOT applied - call <see cref="ExecuteAsync"/> for the full pipeline,
    /// or append <c>.Skip(...).Take(...)</c> yourself after adding additional clauses.
    /// </summary>
    /// <exception cref="UnauthorizedFieldException">
    /// Thrown when the query references a field not registered in this translator.
    /// </exception>
    public IQueryable<T> Apply(IQueryable<T> source, DataQuery query)
    {
        ValidateQuery(query);

        if (query.Filter is not null)
            source = source.Where(FilterExpressionBuilder.Build<T>(query.Filter, _columns, _customOps));

        source = SortExpressionBuilder.Apply(source, query.Sort, _columns);

        return source;
    }

    /// <summary>
    /// Executes the full pipeline: filter → sort → count → paginate → return <see cref="DataPage{T}"/>.
    /// Runs two database queries: one for the total count, one for the page items.
    /// </summary>
    /// <param name="source">
    /// The base queryable to query against. Apply your own tenant filters, soft-delete
    /// clauses, and <c>Include</c> calls here before passing in.
    /// </param>
    /// <param name="query">The grid query.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A page of items and the total matching count.</returns>
    public async Task<DataPage<T>> ExecuteAsync(
        IQueryable<T> source,
        DataQuery query,
        CancellationToken ct = default)
    {
        var q = Apply(source, query);
        var total = await q.CountAsync(ct);
        var items = await q
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(ct);

        return new DataPage<T>(items, total);
    }

    private void ValidateQuery(DataQuery query)
    {
        if (query.Filter is not null)
            ValidateNode(query.Filter);

        foreach (var sort in query.Sort)
        {
            if (!_columns.ContainsKey(sort.Field))
                throw new UnauthorizedFieldException(sort.Field);
        }

        if (query.Columns is not null)
        {
            foreach (var col in query.Columns)
            {
                if (!_columns.ContainsKey(col))
                    throw new UnauthorizedFieldException(col);
            }
        }
    }

    private void ValidateNode(FilterNode node)
    {
        switch (node)
        {
            case FilterAnd and:
                foreach (var child in and.Children) ValidateNode(child);
                break;
            case FilterOr or:
                foreach (var child in or.Children) ValidateNode(child);
                break;
            case FilterNot not:
                ValidateNode(not.Child);
                break;
            case FilterCondition condition:
                if (!_columns.ContainsKey(condition.Field))
                    throw new UnauthorizedFieldException(condition.Field);
                break;
        }
    }
}
