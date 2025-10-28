using BugStore.Api.Features.Products.DeleteProduct;
using BugStore.Api.Exceptions;
using BugStore.Api.Repositories;
using BugStore.Data;
using BugStore.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BugStore.Test.Handlers.Products;

public class DeleteProductHandlerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ProductRepository _repository;

    public DeleteProductHandlerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new ProductRepository(_context);
    }

    [Fact]
    public async Task HandleAsync_ShouldDeleteProduct_WhenExists()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _context.Products.Add(new Product
        {
            Id = productId,
            Title = "To Delete",
            Description = "Description",
            Slug = "to-delete",
            Price = 100.00m
        });
        await _context.SaveChangesAsync();

        var validator = new DeleteProductValidator();
        var handler = new DeleteProductHandler(_repository, validator);
        var request = new DeleteProductRequest { Id = productId };

        // Act
        var result = await handler.HandleAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productId, result.Id);
        
        var product = await _context.Products.FindAsync(productId);
        Assert.Null(product);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowNotFoundException_WhenProductNotFound()
    {
        // Arrange
        var validator = new DeleteProductValidator();
        var handler = new DeleteProductHandler(_repository, validator);
        var request = new DeleteProductRequest { Id = Guid.NewGuid() };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => handler.HandleAsync(request));
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowArgumentException_WhenInvalidId()
    {
        // Arrange
        var validator = new DeleteProductValidator();
        var handler = new DeleteProductHandler(_repository, validator);
        var request = new DeleteProductRequest { Id = Guid.Empty };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => handler.HandleAsync(request));
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}

