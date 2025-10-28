using BugStore.Api.Exceptions;
using BugStore.Api.Repositories.Interfaces;
using FluentValidation;

namespace BugStore.Api.Features.Customers.DeleteCustomer;

public class DeleteCustomerHandler
{
    private readonly ICustomerRepository _repository;
    private readonly AbstractValidator<DeleteCustomerRequest> _validator;

    public DeleteCustomerHandler(ICustomerRepository repository, AbstractValidator<DeleteCustomerRequest> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<DeleteCustomerResponse> HandleAsync(DeleteCustomerRequest request)
    {
        // Validação
        if (!await ValidationAsync(request))
            throw new ArgumentException("ID Inválido");

        var deleted = await _repository.DeleteAsync(request.Id);

        if (!deleted)
            throw new NotFoundException($"Customer com ID {request.Id} não foi removido");

        return new DeleteCustomerResponse
        {
            Id = request.Id,
            Message = "Customer deletado com sucesso"
        };
    }

    private async Task<bool> ValidationAsync(DeleteCustomerRequest request)
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
