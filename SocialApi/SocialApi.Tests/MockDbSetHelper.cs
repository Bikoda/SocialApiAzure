using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using SocialApi.Data;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

public static class MockDbSetHelper
{
    public static Mock<DbSet<T>> CreateMockDbSet<T>(IEnumerable<T> data) where T : class
    {
        var queryableData = data.AsQueryable();

        var mockSet = new Mock<DbSet<T>>();
        mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryableData.Provider);
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryableData.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

        // Mock async operations
        mockSet.As<IAsyncEnumerable<T>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<T>(queryableData.GetEnumerator()));
        mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<T>(queryableData.Provider));

        // Mocking async Count method using LINQ method directly
        mockSet.As<IQueryable<T>>()
            .Setup(m => m.CountAsync(It.IsAny<CancellationToken>()))
            .Returns((CancellationToken cancellationToken) => Task.FromResult(queryableData.Count()));

        return mockSet;
    }

    public static void SetupFindAsync<T>(Mock<DbSet<T>> mockSet, IEnumerable<T> data) where T : class
    {
        mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
            .ReturnsAsync((object[] ids) =>
            {
                var id = ids[0]; // Assuming the first parameter is the primary key
                return data.FirstOrDefault(x => GetPrimaryKeyValue(x).Equals(id));
            });
    }

    private static object GetPrimaryKeyValue<T>(T entity)
    {
        var keyProperty = typeof(T).GetProperties().FirstOrDefault(p => p.Name.EndsWith("Id"));
        if (keyProperty == null)
        {
            throw new InvalidOperationException($"No primary key property found for type {typeof(T).Name}. Ensure a property ending with 'Id' exists.");
        }
        return keyProperty.GetValue(entity);
    }

    // New helper method to create an in-memory DbContext for tests
    public static WebSocialDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<WebSocialDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique database per test
            .Options;

        return new WebSocialDbContext(options); // Return a new instance of WebSocialDbContext
    }
}

public class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
{
    private readonly IQueryProvider _inner;

    public TestAsyncQueryProvider(IQueryProvider inner)
    {
        _inner = inner;
    }

    public IQueryable CreateQuery(Expression expression)
    {
        return new TestAsyncEnumerable<TEntity>(expression);
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        return new TestAsyncEnumerable<TElement>(expression);
    }

    public object Execute(Expression expression)
    {
        return _inner.Execute(expression);
    }

    public TResult Execute<TResult>(Expression expression)
    {
        return _inner.Execute<TResult>(expression);
    }

    public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
    {
        var executionResult = Execute(expression);
        return new TestAsyncEnumerable<TResult>(new[] { (TResult)executionResult });
    }

    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
    {
        var executionResult = Execute(expression);
        return Task.FromResult((TResult)executionResult).Result;
    }
}

public class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
    public TestAsyncEnumerable(Expression expression) : base(expression) { }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
    }

    IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
}

public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _inner;

    public TestAsyncEnumerator(IEnumerator<T> inner)
    {
        _inner = inner;
    }

    public ValueTask DisposeAsync()
    {
        _inner.Dispose();
        return ValueTask.CompletedTask;
    }

    public ValueTask<bool> MoveNextAsync()
    {
        return new ValueTask<bool>(_inner.MoveNext());
    }

    public T Current => _inner.Current;
}
