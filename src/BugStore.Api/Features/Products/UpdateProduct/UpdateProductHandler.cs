using BugStore.Api.Exceptions;
using BugStore.Api.Repositories.Interfaces;
using FluentValidation;

namespace BugStore.Api.Features.Products.UpdateProduct;

public class UpdateProductHandler
{
    private readonly IProductRepository _repository;
    private readonly AbstractValidator<UpdateProductRequest> _validator;

    public UpdateProductHandler(IProductRepository repository, AbstractValidator<UpdateProductRequest> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<UpdateProductResponse> HandleAsync(UpdateProductRequest request)
    {
        // Validação
        if (!await ValidationAsync(request))
            throw new ArgumentException("Dados do produto inválidos.");

        // Buscar product
        var product = await _repository.GetByIdAsync(request.Id);
        if (product == null)
            throw new NotFoundException($"Product com ID {request.Id} não foi encontrado");

        // Atualizar
        product.Title = request.Title;
        product.Description = request.Description;
        product.Slug = request.Slug;
        product.Price = request.Price;

        await _repository.UpdateAsync(product);

        return new UpdateProductResponse
        {
            Id = product.Id,
            Message = "Product atualizado com sucesso"
        };
    }

    private async Task<bool> ValidationAsync(UpdateProductRequest request)
    {
        var result = await _validator.ValidateAsync(request);
        
        if (!result.IsValid)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.ErrorMessage));
            return false;
        }

        return true;
    }
}
