using BugStore.Api.Features.Orders.GetOrderById;
using BugStore.Api.Repositories;
using BugStore.Data;
using BugStore.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BugStore.Test.Handlers.Orders;

public class GetOrderByIdHandlerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly OrderRepository _repository;

    public GetOrderByIdHandlerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new OrderRepository(_context);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnOrder_WhenExists()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var productId1 = Guid.NewGuid();
        var productId2 = Guid.NewGuid();

        var customer = new Customer
        {
            Id = customerId,
            Name = "Test Customer",
            Email = "test@example.com",
            Phone = "123",
            BirthDate = DateTime.Now.AddYears(-30)
        };

        var product1 = new Product
        {
            Id = productId1,
            Title = "Product 1",
            Description = "Desc 1",
            Slug = "product-1",
            Price = 100m
        };

        var product2 = new Product
        {
            Id = productId2,
            Title = "Product 2",
            Description = "Desc 2",
            Slug = "product-2",
            Price = 200m
        };

        _context.Customers.Add(customer);
        _context.Products.Add(product1);
        _context.Products.Add(product2);

        var order = new Order
        {
            Id = orderId,
            CustomerId = customerId,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            Lines = new List<OrderLine>
            {
                new OrderLine { Id = Guid.NewGuid(), OrderId = orderId, ProductId = productId1, Quantity = 2, Total = 200m },
                new OrderLine { Id = Guid.NewGuid(), OrderId = orderId, ProductId = productId2, Quantity = 1, Total = 200m }
            }
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        var handler = new GetOrderByIdHandler(_repository);
        var query = new GetOrderByIdQuery { Id = orderId };

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(orderId, result.Id);
        Assert.Equal(customerId, result.CustomerId);
        Assert.Equal("Test Customer", result.CustomerName);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(400m, result.Total);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        var handler = new GetOrderByIdHandler(_repository);
        var query = new GetOrderByIdQuery { Id = Guid.NewGuid() };

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCorrectOrderData()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var customer = new Customer
        {
            Id = customerId,
            Name = "John Doe",
            Email = "john@example.com",
            Phone = "1234567890",
            BirthDate = DateTime.Now.AddYears(-30)
        };

        var product = new Product
        {
            Id = productId,
            Title = "Test Product",
            Description = "Description",
            Slug = "test-product",
            Price = 150m
        };

        _context.Customers.Add(customer);
        _context.Products.Add(product);

        var createdAt = DateTime.Now;
        var order = new Order
        {
            Id = orderId,
            CustomerId = customerId,
            CreatedAt = createdAt,
            UpdatedAt = createdAt,
            Lines = new List<OrderLine>
            {
                new OrderLine { Id = Guid.NewGuid(), OrderId = orderId, ProductId = productId, Quantity = 3, Total = 450m }
            }
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        var handler = new GetOrderByIdHandler(_repository);
        var query = new GetOrderByIdQuery { Id = orderId };

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John Doe", result.CustomerName);
        Assert.Single(result.Items);
        Assert.Equal("Test Product", result.Items[0].ProductTitle);
        Assert.Equal(3, result.Items[0].Quantity);
        Assert.Equal(150m, result.Items[0].Price);
        Assert.Equal(450m, result.Items[0].Total);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}

