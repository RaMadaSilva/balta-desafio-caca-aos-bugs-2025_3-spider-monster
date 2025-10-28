using BugStore.Api.Features.Customers.GetCustomers;
using BugStore.Api.Repositories;
using BugStore.Data;
using BugStore.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BugStore.Test.Handlers.Customers;

public class GetCustomersHandlerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly CustomerRepository _repository;

    public GetCustomersHandlerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new CustomerRepository(_context);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnAllCustomers()
    {
        // Arrange
        _context.Customers.AddRange(
            new Customer { Id = Guid.NewGuid(), Name = "Customer 1", Email = "c1@example.com", Phone = "123", BirthDate = DateTime.Now.AddYears(-30) },
            new Customer { Id = Guid.NewGuid(), Name = "Customer 2", Email = "c2@example.com", Phone = "456", BirthDate = DateTime.Now.AddYears(-25) },
            new Customer { Id = Guid.NewGuid(), Name = "Customer 3", Email = "c3@example.com", Phone = "789", BirthDate = DateTime.Now.AddYears(-35) }
        );
        await _context.SaveChangesAsync();

        var handler = new GetCustomersHandler(_repository);
        var query = new GetCustomersQuery();

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.All(result, c => Assert.NotEqual(Guid.Empty, c.Id));
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnEmptyList_WhenNoCustomers()
    {
        // Arrange
        var handler = new GetCustomersHandler(_repository);
        var query = new GetCustomersQuery();

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCustomersInOrder()
    {
        // Arrange
        _context.Customers.AddRange(
            new Customer { Id = Guid.NewGuid(), Name = "Zebra", Email = "z@example.com", Phone = "123", BirthDate = DateTime.Now.AddYears(-30) },
            new Customer { Id = Guid.NewGuid(), Name = "Alpha", Email = "a@example.com", Phone = "123", BirthDate = DateTime.Now.AddYears(-30) },
            new Customer { Id = Guid.NewGuid(), Name = "Beta", Email = "b@example.com", Phone = "123", BirthDate = DateTime.Now.AddYears(-30) }
        );
        await _context.SaveChangesAsync();

        var handler = new GetCustomersHandler(_repository);
        var query = new GetCustomersQuery();

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.Equal("Alpha", result[0].Name);
        Assert.Equal("Beta", result[1].Name);
        Assert.Equal("Zebra", result[2].Name);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}

