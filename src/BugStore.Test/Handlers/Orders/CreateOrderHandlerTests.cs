using BugStore.Api.Features.Orders.CreateOrder;
using BugStore.Api.Exceptions;
using BugStore.Api.Repositories;
using BugStore.Data;
using BugStore.Models;
using Microsoft.EntityFrameworkCore;

namespace BugStore.Test.Handlers.Orders;

public class CreateOrderHandlerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly OrderRepository _orderRepository;
    private readonly CustomerRepository _customerRepository;
    private readonly ProductRepository _productRepository;

    public CreateOrderHandlerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _orderRepository = new OrderRepository(_context);
        _customerRepository = new CustomerRepository(_context);
        _productRepository = new ProductRepository(_context);
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateOrder_WhenValid()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var productId1 = Guid.NewGuid();
        var productId2 = Guid.NewGuid();

        _context.Customers.Add(new Customer
        {
            Id = customerId,
            Name = "Test Customer",
            Email = "test@example.com",
            Phone = "1234567890",
            BirthDate = DateTime.Now.AddYears(-30)
        });

        _context.Products.Add(new Product
        {
            Id = productId1,
            Title = "Product 1",
            Description = "Desc 1",
            Slug = "product-1",
            Price = 100m
        });

        _context.Products.Add(new Product
        {
            Id = productId2,
            Title = "Product 2",
            Description = "Desc 2",
            Slug = "product-2",
            Price = 200m
        });

        await _context.SaveChangesAsync();

        var validator = new CreateOrderValidator();
        var handler = new CreateOrderHandler(_orderRepository, _customerRepository, _productRepository, validator);
        var request = new CreateOrderRequest
        {
            CustomerId = customerId,
            Items = new List<OrderLineItem>
            {
                new OrderLineItem { ProductId = productId1, Quantity = 2 },
                new OrderLineItem { ProductId = productId2, Quantity = 3 }
            }
        };

        // Act
        var result = await handler.HandleAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(customerId, result.CustomerId);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(800m, result.Total); 
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowNotFoundException_WhenCustomerNotFound()
    {
        // Arrange
        var validator = new CreateOrderValidator();
        var handler = new CreateOrderHandler(_orderRepository, _customerRepository, _productRepository, validator);
        var request = new CreateOrderRequest
        {
            CustomerId = Guid.NewGuid(),
            Items = new List<OrderLineItem>
            {
                new OrderLineItem { ProductId = Guid.NewGuid(), Quantity = 1 }
            }
        };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => handler.HandleAsync(request));
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowInvalidOperationException_WhenProductNotFound()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        _context.Customers.Add(new Customer
        {
            Id = customerId,
            Name = "Test Customer",
            Email = "test@example.com",
            Phone = "123",
            BirthDate = DateTime.Now.AddYears(-30)
        });
        await _context.SaveChangesAsync();

        var validator = new CreateOrderValidator();
        var handler = new CreateOrderHandler(_orderRepository, _customerRepository, _productRepository, validator);
        var request = new CreateOrderRequest
        {
            CustomerId = customerId,
            Items = new List<OrderLineItem>
            {
                new OrderLineItem { ProductId = Guid.NewGuid(), Quantity = 1 } // Product doesn't exist
            }
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleAsync(request));
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowArgumentException_WhenInvalidData()
    {
        // Arrange
        var validator = new CreateOrderValidator();
        var handler = new CreateOrderHandler(_orderRepository, _customerRepository, _productRepository, validator);
        var request = new CreateOrderRequest
        {
            CustomerId = Guid.Empty, // Invalid
            Items = new List<OrderLineItem>() // Empty
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => handler.HandleAsync(request));
    }

    [Fact]
    public async Task HandleAsync_ShouldCalculateTotalCorrectly()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        _context.Customers.Add(new Customer
        {
            Id = customerId,
            Name = "Test Customer",
            Email = "test@example.com",
            Phone = "123",
            BirthDate = DateTime.Now.AddYears(-30)
        });

        _context.Products.Add(new Product
        {
            Id = productId,
            Title = "Product",
            Description = "Desc",
            Slug = "product",
            Price = 50m
        });

        await _context.SaveChangesAsync();

        var validator = new CreateOrderValidator();
        var handler = new CreateOrderHandler(_orderRepository, _customerRepository, _productRepository, validator);
        var request = new CreateOrderRequest
        {
            CustomerId = customerId,
            Items = new List<OrderLineItem>
            {
                new OrderLineItem { ProductId = productId, Quantity = 5 }
            }
        };

        // Act
        var result = await handler.HandleAsync(request);

        // Assert
        Assert.Equal(250m, result.Total); // 50 * 5
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}

