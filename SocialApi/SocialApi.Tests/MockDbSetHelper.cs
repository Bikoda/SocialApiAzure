using Microsoft.EntityFrameworkCore;
using Moq;
using System.Linq.Expressions;
using System.Collections.Generic;
using SocialApi.Data;

public static class MockDbSetHelper
{
    public static Mock<DbSet<T>> CreateMockDbSet<T>(IQueryable<T> data) where T : class
    {
        var mockSet = new Mock<DbSet<T>>();

        // Mock IQueryable interface
        mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

        // Mock Add method to modify data directly
        mockSet.Setup(m => m.Add(It.IsAny<T>())).Callback<T>((entity) =>
        {
            var list = data.ToList();
            list.Add(entity);
            data = list.AsQueryable();  // Update the queryable collection
        });

        // Mock AddRange method to modify data directly
        mockSet.Setup(m => m.AddRange(It.IsAny<IEnumerable<T>>())).Callback<IEnumerable<T>>((entities) =>
        {
            var list = data.ToList();
            list.AddRange(entities);
            data = list.AsQueryable();  // Update the queryable collection
        });

        // Mock Remove method to modify data directly
        mockSet.Setup(m => m.Remove(It.IsAny<T>())).Callback<T>((entity) =>
        {
            var list = data.ToList();
            list.Remove(entity);
            data = list.AsQueryable();  // Update the queryable collection
        });

        // Mock SaveChanges on the DbContext, not DbSet
        var mockDbContext = new Mock<WebSocialDbContext>(Mock.Of<DbContextOptions<WebSocialDbContext>>());
        mockDbContext.Setup(m => m.SaveChanges()).Returns(1); // Simulate 1 record saved

        return mockSet;
    }
}