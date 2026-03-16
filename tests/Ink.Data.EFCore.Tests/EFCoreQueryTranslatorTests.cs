using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Ink.Data.EFCore;
using Ink.Data.Queries;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ink.Data.EFCore.Tests;

// ── Test fixtures ────────────────────────────────────────────────────────────

public sealed class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public bool Active { get; set; }
    public string? Email { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
    public DbSet<User> Users => Set<User>();
}

// ── Tests ────────────────────────────────────────────────────────────────────

// EF Core InMemory has a known string-ordering bug in 9.x - string OrderBy returns
// non-alphabetical results. SQLite in-memory is used so sort behavior matches a real
// database provider.
public class EFCoreQueryTranslatorTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly TestDbContext _db;
    private readonly EFCoreQueryTranslator<User> _translator;

    public EFCoreQueryTranslatorTests()
    {
        // Keep the connection alive for the test lifetime - SQLite in-memory drops the
        // database when all connections close.
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(_connection)
            .Options;

        _db = new TestDbContext(options);
        _db.Database.EnsureCreated();

        _db.Users.AddRange(
            new User { Id = 1, Name = "Alice",   Age = 30, Active = true,  Email = "alice@example.com", CreatedAt = new DateTime(2024, 1, 1) },
            new User { Id = 2, Name = "Bob",     Age = 25, Active = true,  Email = "bob@example.com",   CreatedAt = new DateTime(2024, 2, 1) },
            new User { Id = 3, Name = "Charlie", Age = 35, Active = false, Email = null,                CreatedAt = new DateTime(2024, 3, 1) },
            new User { Id = 4, Name = "Diana",   Age = 28, Active = true,  Email = "diana@corp.com",    CreatedAt = new DateTime(2024, 4, 1) },
            new User { Id = 5, Name = "Eve",     Age = 22, Active = false, Email = "eve@example.com",   CreatedAt = new DateTime(2024, 5, 1) });
        _db.SaveChanges();

        _translator = new EFCoreQueryTranslator<User>()
            .Column(x => x.Name)
            .Column(x => x.Age)
            .Column(x => x.Active)
            .Column(x => x.Email)
            .Column(x => x.CreatedAt);
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }

    // ── Filtering ────────────────────────────────────────────────────────────

    [Fact]
    public async Task Filter_Contains_ReturnsMatchingRows()
    {
        var query = DataGridQuery.Default with
        {
            Filter = new FilterCondition("name", FilterOp.Contains, ["li"]),
        };

        var result = await _translator.ExecuteAsync(_db.Users, query);

        Assert.Equal(2, result.TotalCount);
        Assert.Contains(result.Items, u => u.Name == "Alice");
        Assert.Contains(result.Items, u => u.Name == "Charlie");
    }

    [Fact]
    public async Task Filter_GreaterThan_ReturnsMatchingRows()
    {
        var query = DataGridQuery.Default with
        {
            Filter = new FilterCondition("age", FilterOp.GreaterThan, [28]),
        };

        var result = await _translator.ExecuteAsync(_db.Users, query);

        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Items, u => Assert.True(u.Age > 28));
    }

    [Fact]
    public async Task Filter_Between_ReturnsRowsInRange()
    {
        var query = DataGridQuery.Default with
        {
            Filter = new FilterCondition("age", FilterOp.Between, [25, 30]),
        };

        var result = await _translator.ExecuteAsync(_db.Users, query);

        Assert.Equal(3, result.TotalCount);
        Assert.All(result.Items, u => Assert.True(u.Age >= 25 && u.Age <= 30));
    }

    [Fact]
    public async Task Filter_Equal_Bool_ReturnsMatchingRows()
    {
        var query = DataGridQuery.Default with
        {
            Filter = new FilterCondition("active", FilterOp.Equal, [true]),
        };

        var result = await _translator.ExecuteAsync(_db.Users, query);

        Assert.Equal(3, result.TotalCount);
        Assert.All(result.Items, u => Assert.True(u.Active));
    }

    [Fact]
    public async Task Filter_IsNull_ReturnsNullRows()
    {
        var query = DataGridQuery.Default with
        {
            Filter = new FilterCondition("email", FilterOp.IsNull, []),
        };

        var result = await _translator.ExecuteAsync(_db.Users, query);

        Assert.Equal(1, result.TotalCount);
        Assert.Equal("Charlie", result.Items[0].Name);
    }

    [Fact]
    public async Task Filter_In_ReturnsMatchingRows()
    {
        var query = DataGridQuery.Default with
        {
            Filter = new FilterCondition("name", FilterOp.In, ["Alice", "Eve"]),
        };

        var result = await _translator.ExecuteAsync(_db.Users, query);

        Assert.Equal(2, result.TotalCount);
        Assert.Contains(result.Items, u => u.Name == "Alice");
        Assert.Contains(result.Items, u => u.Name == "Eve");
    }

    [Fact]
    public async Task Filter_And_CombinesConditions()
    {
        var query = DataGridQuery.Default with
        {
            Filter = new FilterAnd([
                new FilterCondition("active", FilterOp.Equal, [true]),
                new FilterCondition("age", FilterOp.GreaterThan, [26]),
            ]),
        };

        var result = await _translator.ExecuteAsync(_db.Users, query);

        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Items, u => Assert.True(u.Active && u.Age > 26));
    }

    [Fact]
    public async Task Filter_Or_ExpandsResults()
    {
        var query = DataGridQuery.Default with
        {
            Filter = new FilterOr([
                new FilterCondition("name", FilterOp.Equal, ["Alice"]),
                new FilterCondition("name", FilterOp.Equal, ["Eve"]),
            ]),
        };

        var result = await _translator.ExecuteAsync(_db.Users, query);

        Assert.Equal(2, result.TotalCount);
    }

    [Fact]
    public async Task Filter_Not_InvertsCondition()
    {
        var query = DataGridQuery.Default with
        {
            Filter = new FilterNot(new FilterCondition("active", FilterOp.Equal, [true])),
        };

        var result = await _translator.ExecuteAsync(_db.Users, query);

        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Items, u => Assert.False(u.Active));
    }

    // ── Sorting ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task Sort_ByNameAscending_ReturnsAlphabetical()
    {
        var query = DataGridQuery.Default with
        {
            Sort = [new SortDescriptor("name", SortDirection.Ascending)],
        };

        var result = await _translator.ExecuteAsync(_db.Users, query);

        var names = result.Items.Select(u => u.Name).ToList();
        Assert.Equal(["Alice", "Bob", "Charlie", "Diana", "Eve"], names);
    }

    [Fact]
    public async Task Sort_ByAgeDescending_ReturnsSortedByAge()
    {
        var query = DataGridQuery.Default with
        {
            Sort = [new SortDescriptor("age", SortDirection.Descending)],
        };

        var result = await _translator.ExecuteAsync(_db.Users, query);

        var ages = result.Items.Select(u => u.Age).ToList();
        Assert.Equal(ages.OrderByDescending(a => a).ToList(), ages);
    }

    [Fact]
    public async Task Sort_MultiColumn_AppliesInOrder()
    {
        var query = DataGridQuery.Default with
        {
            Sort = [
                new SortDescriptor("active", SortDirection.Descending),
                new SortDescriptor("name", SortDirection.Ascending),
            ],
        };

        var result = await _translator.ExecuteAsync(_db.Users, query);

        // Active users come first (true > false), then sorted by name within each group
        var activeNames = result.Items.Where(u => u.Active).Select(u => u.Name).ToList();
        Assert.Equal(activeNames.OrderBy(n => n).ToList(), activeNames);
    }

    // ── Pagination ───────────────────────────────────────────────────────────

    [Fact]
    public async Task Paginate_ReturnsCorrectPage()
    {
        var query = new DataGridQuery(null, [new SortDescriptor("name", SortDirection.Ascending)], null, Page: 2, PageSize: 2);

        var result = await _translator.ExecuteAsync(_db.Users, query);

        Assert.Equal(5, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal("Charlie", result.Items[0].Name);
        Assert.Equal("Diana", result.Items[1].Name);
    }

    [Fact]
    public async Task Paginate_LastPage_ReturnsRemainingItems()
    {
        var query = new DataGridQuery(null, [new SortDescriptor("name", SortDirection.Ascending)], null, Page: 3, PageSize: 2);

        var result = await _translator.ExecuteAsync(_db.Users, query);

        Assert.Equal(5, result.TotalCount);
        Assert.Single(result.Items);
        Assert.Equal("Eve", result.Items[0].Name);
    }

    // ── Security ─────────────────────────────────────────────────────────────

    [Fact]
    public void UnregisteredField_InFilter_ThrowsUnauthorizedFieldException()
    {
        var query = DataGridQuery.Default with
        {
            Filter = new FilterCondition("passwordHash", FilterOp.Equal, ["secret"]),
        };

        Assert.Throws<UnauthorizedFieldException>(() => _translator.Apply(_db.Users, query));
    }

    [Fact]
    public void UnregisteredField_InSort_ThrowsUnauthorizedFieldException()
    {
        var query = DataGridQuery.Default with
        {
            Sort = [new SortDescriptor("passwordHash", SortDirection.Ascending)],
        };

        Assert.Throws<UnauthorizedFieldException>(() => _translator.Apply(_db.Users, query));
    }

    // ── Custom operators ─────────────────────────────────────────────────────

    [Fact]
    public async Task HandleOp_CustomOp_IsInvokedAndFiltersCorrectly()
    {
        // Register a custom "endsWith" implemented differently to prove the handler is called
        var notContains = new FilterOp("notContains");

        var translator = new EFCoreQueryTranslator<User>()
            .Column(x => x.Name)
            .HandleOp(notContains, (member, _, values) =>
            {
                var method = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!;
                var value = Expression.Constant(values[0]?.ToString(), typeof(string));
                return Expression.Not(Expression.Call(member, method, value));
            });

        var query = DataGridQuery.Default with
        {
            Filter = new FilterCondition("name", notContains, ["li"]),
        };

        var result = await translator.ExecuteAsync(_db.Users, query);

        // Should return everyone whose name does NOT contain "li" (3 of 5)
        Assert.Equal(3, result.TotalCount);
        Assert.DoesNotContain(result.Items, u => u.Name.Contains("li", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void HandleOp_UnknownBuiltInOp_AndNoCustomHandler_Throws()
    {
        var query = DataGridQuery.Default with
        {
            Filter = new FilterCondition("name", new FilterOp("unknownOp"), ["x"]),
        };

        // Must register "unknownOp" via HandleOp - otherwise should throw
        Assert.Throws<ArgumentException>(() => _translator.Apply(_db.Users, query));
    }
}
