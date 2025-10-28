using BugStore.Api.Features.Products.GetProductById;
using BugStore.Api.Repositories;
using BugStore.Data;
using BugStore.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BugStore.Test.Handlers.Products;

public class GetProductByIdHandlerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ProductRepository _repository;

    public GetProductByIdHandlerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new ProductRepository(_context);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnProduct_WhenExists()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _context.Products.Add(new Product
        {
            Id = productId,
            Title = "Test Product",
            Description = "Test Description",
            Slug = "test-product",
            Price = 99.99m
        });
        await _context.SaveChangesAsync();

        var handler = new GetProductByIdHandler(_repository);
        var query = new GetProductByIdQuery { Id = productId };

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productId, result.Id);
        Assert.Equal("Test Product", result.Title);
        Assert.Equal("test-product", result.Slug);
        Assert.Equal(99.99m, result.Price);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        var handler = new GetProductByIdHandler(_repository);
        var query = new GetProductByIdQuery { Id = Guid.NewGuid() };

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCorrectProductData()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _context.Products.Add(new Product
        {
            Id = productId,
            Title = "Special Product",
            Description = "Special Description",
            Slug = "special-product",
            Price = 250.50m
        });
        await _context.SaveChangesAsync();

        var handler = new GetProductByIdHandler(_repository);
        var query = new GetProductByIdQuery { Id = productId };

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Special Product", result.Title);
        Assert.Equal("Special Description", result.Description);
        Assert.Equal("special-product", result.Slug);
        Assert.Equal(250.50m, result.Price);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}

