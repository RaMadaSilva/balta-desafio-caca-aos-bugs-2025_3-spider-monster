using BugStore.Api.Features.Customers.GetByIdCustomer;
using BugStore.Api.Repositories;
using BugStore.Data;
using BugStore.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BugStore.Test.Handlers.Customers;

public class GetByIdCustomerHandlerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly CustomerRepository _repository;

    public GetByIdCustomerHandlerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new CustomerRepository(_context);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCustomer_WhenExists()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        _context.Customers.Add(new Customer
        {
            Id = customerId,
            Name = "Raul Silva",
            Email = "raul@example.com",
            Phone = "9234567890",
            BirthDate = DateTime.Now.AddYears(-30)
        });
        await _context.SaveChangesAsync();

        var handler = new GetByIdCustomerHandler(_repository);
        var query = new GetByIdCustomerQuery { Id = customerId };

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(customerId, result.Id);
        Assert.Equal("Raul Silva", result.Name);
        Assert.Equal("raul@example.com", result.Email);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        var handler = new GetByIdCustomerHandler(_repository);
        var query = new GetByIdCustomerQuery { Id = Guid.NewGuid() };

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCorrectCustomerData()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var birthDate = DateTime.Now.AddYears(-28);
        _context.Customers.Add(new Customer
        {
            Id = customerId,
            Name = "Test Customer",
            Email = "test@example.com",
            Phone = "9876543210",
            BirthDate = birthDate
        });
        await _context.SaveChangesAsync();

        var handler = new GetByIdCustomerHandler(_repository);
        var query = new GetByIdCustomerQuery { Id = customerId };

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Customer", result.Name);
        Assert.Equal("test@example.com", result.Email);
        Assert.Equal("9876543210", result.Phone);
        Assert.Equal(birthDate, result.BirthDate);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}

