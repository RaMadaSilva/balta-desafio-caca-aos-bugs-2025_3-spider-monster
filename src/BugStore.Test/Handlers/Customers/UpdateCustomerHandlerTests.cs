using BugStore.Api.Features.Customers.UpdateCustomer;
using BugStore.Api.Exceptions;
using BugStore.Api.Repositories;
using BugStore.Data;
using BugStore.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BugStore.Test.Handlers.Customers;

public class UpdateCustomerHandlerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly CustomerRepository _repository;

    public UpdateCustomerHandlerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new CustomerRepository(_context);
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateCustomer_WhenExists()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        _context.Customers.Add(new Customer
        {
            Id = customerId,
            Name = "Original Name",
            Email = "original@example.com",
            Phone = "1234567890",
            BirthDate = DateTime.Now.AddYears(-30)
        });
        await _context.SaveChangesAsync();

        var validator = new UpdateCustomerValidator();
        var handler = new UpdateCustomerHandler(_repository, validator);
        var request = new UpdateCustomerRequest
        {
            Id = customerId,
            Name = "Updated Name",
            Email = "updated@example.com",
            Phone = "9876543210",
            BirthDate = DateTime.Now.AddYears(-29)
        };

        // Act
        var result = await handler.HandleAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(customerId, result.Id);
        
        var updatedCustomer = await _context.Customers.FindAsync(customerId);
        Assert.NotNull(updatedCustomer);
        Assert.Equal("Updated Name", updatedCustomer.Name);
        Assert.Equal("updated@example.com", updatedCustomer.Email);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowNotFoundException_WhenCustomerNotFound()
    {
        // Arrange
        var validator = new UpdateCustomerValidator();
        var handler = new UpdateCustomerHandler(_repository, validator);
        var request = new UpdateCustomerRequest
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            Email = "test@example.com",
            Phone = "123",
            BirthDate = DateTime.Now.AddYears(-30)
        };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => handler.HandleAsync(request));
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowArgumentException_WhenInvalidData()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        _context.Customers.Add(new Customer
        {
            Id = customerId,
            Name = "Test",
            Email = "test@example.com",
            Phone = "123",
            BirthDate = DateTime.Now.AddYears(-30)
        });
        await _context.SaveChangesAsync();

        var validator = new UpdateCustomerValidator();
        var handler = new UpdateCustomerHandler(_repository, validator);
        var request = new UpdateCustomerRequest
        {
            Id = customerId,
            Name = "", // Invalid
            Email = "invalid-email", // Invalid
            Phone = "123",
            BirthDate = DateTime.Now.AddYears(-30)
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => handler.HandleAsync(request));
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}

