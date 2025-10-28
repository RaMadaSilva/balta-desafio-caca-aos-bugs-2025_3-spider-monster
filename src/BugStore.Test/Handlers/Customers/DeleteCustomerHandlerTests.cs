using BugStore.Api.Features.Customers.DeleteCustomer;
using BugStore.Api.Exceptions;
using BugStore.Api.Repositories;
using BugStore.Data;
using BugStore.Models;
using Microsoft.EntityFrameworkCore;

namespace BugStore.Test.Handlers.Customers;

public class DeleteCustomerHandlerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly CustomerRepository _repository;

    public DeleteCustomerHandlerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new CustomerRepository(_context);
    }

    [Fact]
    public async Task HandleAsync_ShouldDeleteCustomer_WhenExists()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        _context.Customers.Add(new Customer
        {
            Id = customerId,
            Name = "To Delete",
            Email = "delete@example.com",
            Phone = "1234567890",
            BirthDate = DateTime.Now.AddYears(-30)
        });
        await _context.SaveChangesAsync();

        var validator = new DeleteCustomerValidator();
        var handler = new DeleteCustomerHandler(_repository, validator);
        var request = new DeleteCustomerRequest { Id = customerId };

        // Act
        var result = await handler.HandleAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(customerId, result.Id);
        
        var customer = await _context.Customers.FindAsync(customerId);
        Assert.Null(customer);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowNotFoundException_WhenCustomerNotFound()
    {
        // Arrange
        var validator = new DeleteCustomerValidator();
        var handler = new DeleteCustomerHandler(_repository, validator);
        var request = new DeleteCustomerRequest { Id = Guid.NewGuid() };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => handler.HandleAsync(request));
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowArgumentException_WhenInvalidId()
    {
        // Arrange
        var validator = new DeleteCustomerValidator();
        var handler = new DeleteCustomerHandler(_repository, validator);
        var request = new DeleteCustomerRequest { Id = Guid.Empty };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => handler.HandleAsync(request));
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}

