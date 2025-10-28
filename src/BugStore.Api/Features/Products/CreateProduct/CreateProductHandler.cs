using BugStore.Api.Repositories.Interfaces;
using BugStore.Models;
using FluentValidation;

namespace BugStore.Api.Features.Products.CreateProduct; 

public class CreateProductHandler
{
    private readonly IProductRepository _repository;
    private readonly AbstractValidator<CreateProductRequest> _validator;

    public CreateProductHandler(IProductRepository repository, AbstractValidator<CreateProductRequest> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<CreateProductResponse> HandleAsync(CreateProductRequest request)
    {
        // Validação
        if(!await ValidationAsync(request))
            throw new ArgumentException("Invalid product data.");

        // Criar entidade
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Slug = request.Slug,
            Price = request.Price
        };

        var createdProduct = await _repository.CreateAsync(product);

        // Retornar resposta
        return new CreateProductResponse
        {
            Id = createdProduct.Id,
            Title = createdProduct.Title,
            Description = createdProduct.Description,
            Slug = createdProduct.Slug,
            Price = createdProduct.Price,
            CreatedAt = DateTime.Now
        };
    }

    private async Task<bool> ValidationAsync(CreateProductRequest request)
    {
        var result = await _validator.ValidateAsync(request);
        
        if(!result.IsValid)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.ErrorMessage));
            return false; 
        }

        return true;
    }
}
