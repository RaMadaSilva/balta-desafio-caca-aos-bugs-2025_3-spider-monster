using BugStore.Api.Features.Customers.CreateCustomer;
using BugStore.Api.Repositories;
using BugStore.Data;
using Microsoft.EntityFrameworkCore;

namespace BugStore.Test.Handlers.Customers;

public class CreateCustomerHandlerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly CustomerRepository _repository;

    public CreateCustomerHandlerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new CustomerRepository(_context);
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateCustomer_WhenValid()
    {
        // Arrange
        var validator = new CreateCustomerValidator();
        var handler = new CreateCustomerHandler(_repository, validator);
        var request = new CreateCustomerRequest
        {
            Name = "Raul Silva",
            Email = "raul@gtest.com",
            Phone = "9234567890",
            BirthDate = DateTime.Now.AddYears(-30)
        };

        // Act
        var result = await handler.HandleAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Raul Silva", result.Name);
        Assert.Equal("raul@gtest.com", result.Email);
        Assert.Contains(_context.Customers, c => c.Id == result.Id);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowArgumentException_WhenInvalidData()
    {
        // Arrange
        var validator = new CreateCustomerValidator();
        var handler = new CreateCustomerHandler(_repository, validator);
        var request = new CreateCustomerRequest
        {
            Name = "", 
            Email = "invalid-email",
            Phone = "123",
            BirthDate = DateTime.Now.AddYears(-20)
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => handler.HandleAsync(request));
    }

    [Fact]
    public async Task HandleAsync_ShouldSaveToDatabase_WhenSuccess()
    {
        // Arrange
        var validator = new CreateCustomerValidator();
        var handler = new CreateCustomerHandler(_repository, validator);
        var request = new CreateCustomerRequest
        {
            Name = "Andre Baltier",
            Email = "andre@example.com",
            Phone = "5234567890",
            BirthDate = DateTime.Now.AddYears(-25)
        };

        // Act
        await handler.HandleAsync(request);

        // Assert
        var customer = await _context.Customers.FirstOrDefaultAsync();
        Assert.NotNull(customer);
        Assert.Equal("Andre Baltier", customer.Name);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}

