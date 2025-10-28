using BugStore.Api.Features.Products.CreateProduct;
using BugStore.Api.Repositories;
using BugStore.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BugStore.Test.Handlers.Products;

public class CreateProductHandlerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ProductRepository _repository;

    public CreateProductHandlerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new ProductRepository(_context);
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateProduct_WhenValid()
    {
        // Arrange
        var validator = new CreateProductValidator();
        var handler = new CreateProductHandler(_repository, validator);
        var request = new CreateProductRequest
        {
            Title = "Test Product",
            Description = "Test Description",
            Slug = "test-product",
            Price = 99.99m
        };

        // Act
        var result = await handler.HandleAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Test Product", result.Title);
        Assert.Equal("test-product", result.Slug);
        Assert.Equal(99.99m, result.Price);
        Assert.Contains(_context.Products, p => p.Id == result.Id);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowArgumentException_WhenInvalidData()
    {
        // Arrange
        var validator = new CreateProductValidator();
        var handler = new CreateProductHandler(_repository, validator);
        var request = new CreateProductRequest
        {
            Title = "", // Invalid
            Description = "Test",
            Slug = "", // Invalid
            Price = -10 // Invalid
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => handler.HandleAsync(request));
    }

    [Fact]
    public async Task HandleAsync_ShouldSaveToDatabase_WhenSuccess()
    {
        // Arrange
        var validator = new CreateProductValidator();
        var handler = new CreateProductHandler(_repository, validator);
        var request = new CreateProductRequest
        {
            Title = "My Product",
            Description = "Description here",
            Slug = "my-product",
            Price = 150.00m
        };

        // Act
        await handler.HandleAsync(request);

        // Assert
        var product = await _context.Products.FirstOrDefaultAsync();
        Assert.NotNull(product);
        Assert.Equal("My Product", product.Title);
        Assert.Equal(150.00m, product.Price);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}

