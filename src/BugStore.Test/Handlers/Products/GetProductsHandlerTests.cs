using BugStore.Api.Features.Products.GetProducts;
using BugStore.Api.Repositories;
using BugStore.Data;
using BugStore.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BugStore.Test.Handlers.Products;

public class GetProductsHandlerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ProductRepository _repository;

    public GetProductsHandlerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new ProductRepository(_context);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnAllProducts()
    {
        // Arrange
        _context.Products.AddRange(
            new Product { Id = Guid.NewGuid(), Title = "Product 1", Description = "Desc 1", Slug = "product-1", Price = 100m },
            new Product { Id = Guid.NewGuid(), Title = "Product 2", Description = "Desc 2", Slug = "product-2", Price = 200m },
            new Product { Id = Guid.NewGuid(), Title = "Product 3", Description = "Desc 3", Slug = "product-3", Price = 300m }
        );
        await _context.SaveChangesAsync();

        var handler = new GetProductsHandler(_repository);
        var query = new GetProductsQuery();

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.All(result, p => Assert.NotEqual(Guid.Empty, p.Id));
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnEmptyList_WhenNoProducts()
    {
        // Arrange
        var handler = new GetProductsHandler(_repository);
        var query = new GetProductsQuery();

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnProductsInOrder()
    {
        // Arrange
        _context.Products.AddRange(
            new Product { Id = Guid.NewGuid(), Title = "Zebra Product", Description = "Desc", Slug = "zebra", Price = 100m },
            new Product { Id = Guid.NewGuid(), Title = "Alpha Product", Description = "Desc", Slug = "alpha", Price = 200m },
            new Product { Id = Guid.NewGuid(), Title = "Beta Product", Description = "Desc", Slug = "beta", Price = 300m }
        );
        await _context.SaveChangesAsync();

        var handler = new GetProductsHandler(_repository);
        var query = new GetProductsQuery();

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.Equal("Alpha Product", result[0].Title);
        Assert.Equal("Beta Product", result[1].Title);
        Assert.Equal("Zebra Product", result[2].Title);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}

