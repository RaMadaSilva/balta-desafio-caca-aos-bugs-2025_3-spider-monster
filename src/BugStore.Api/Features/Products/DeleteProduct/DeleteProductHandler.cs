using BugStore.Api.Exceptions;
using BugStore.Api.Repositories.Interfaces;
using FluentValidation;

namespace BugStore.Api.Features.Products.DeleteProduct;

public class DeleteProductHandler
{
    private readonly IProductRepository _repository;
    private readonly AbstractValidator<DeleteProductRequest> _validator;

    public DeleteProductHandler(IProductRepository repository, AbstractValidator<DeleteProductRequest> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<DeleteProductResponse> HandleAsync(DeleteProductRequest request)
    {
        // Validação
        if (!await ValidationAsync(request))
            throw new ArgumentException("ID Inválido");

        var deleted = await _repository.DeleteAsync(request.Id);
        if (!deleted)
            throw new NotFoundException($"Product com ID {request.Id} não foi encontrado");

        return new DeleteProductResponse
        {
            Id = request.Id,
            Message = "Product deletado com sucesso"
        };
    }

    private async Task<bool> ValidationAsync(DeleteProductRequest request)
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
