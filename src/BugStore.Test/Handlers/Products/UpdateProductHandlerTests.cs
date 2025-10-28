using BugStore.Api.Features.Products.UpdateProduct;
using BugStore.Api.Exceptions;
using BugStore.Api.Repositories;
using BugStore.Data;
using BugStore.Models;
using Microsoft.EntityFrameworkCore;

namespace BugStore.Test.Handlers.Products;

public class UpdateProductHandlerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ProductRepository _repository;

    public UpdateProductHandlerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new ProductRepository(_context);
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateProduct_WhenExists()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _context.Products.Add(new Product
        {
            Id = productId,
            Title = "Original Title",
            Description = "Original Description",
            Slug = "original-product",
            Price = 100.00m
        });
        await _context.SaveChangesAsync();

        var validator = new UpdateProductValidator();
        var handler = new UpdateProductHandler(_repository, validator);
        var request = new UpdateProductRequest
        {
            Id = productId,
            Title = "Updated Title",
            Description = "Updated Description",
            Slug = "updated-product",
            Price = 200.00m
        };

        // Act
        var result = await handler.HandleAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productId, result.Id);
        
        var updatedProduct = await _context.Products.FindAsync(productId);
        Assert.NotNull(updatedProduct);
        Assert.Equal("Updated Title", updatedProduct.Title);
        Assert.Equal(200.00m, updatedProduct.Price);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowNotFoundException_WhenProductNotFound()
    {
        // Arrange
        var validator = new UpdateProductValidator();
        var handler = new UpdateProductHandler(_repository, validator);
        var request = new UpdateProductRequest
        {
            Id = Guid.NewGuid(),
            Title = "Test Product",
            Description = "Test",
            Slug = "test-product",
            Price = 50.00m
        };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => handler.HandleAsync(request));
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowArgumentException_WhenInvalidData()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _context.Products.Add(new Product
        {
            Id = productId,
            Title = "Original",
            Description = "Description",
            Slug = "original",
            Price = 100.00m
        });
        await _context.SaveChangesAsync();

        var validator = new UpdateProductValidator();
        var handler = new UpdateProductHandler(_repository, validator);
        var request = new UpdateProductRequest
        {
            Id = productId,
            Title = "", // Invalid
            Description = "Description",
            Slug = "slug",
            Price = -10 // Invalid
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

