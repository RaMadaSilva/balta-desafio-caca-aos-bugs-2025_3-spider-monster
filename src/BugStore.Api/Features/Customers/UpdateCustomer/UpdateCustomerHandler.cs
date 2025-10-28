using BugStore.Api.Exceptions;
using BugStore.Api.Repositories.Interfaces;
using FluentValidation;

namespace BugStore.Api.Features.Customers.UpdateCustomer;

public class UpdateCustomerHandler
{
    private readonly ICustomerRepository _repository;
    private readonly AbstractValidator<UpdateCustomerRequest> _validator;

    public UpdateCustomerHandler(ICustomerRepository repository, AbstractValidator<UpdateCustomerRequest> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<UpdateCustomerResponse> HandleAsync(UpdateCustomerRequest request)
    {
        // Validação
        if (!await ValidationAsync(request))
            throw new ArgumentException("Dados do cliente inválidos.");

        // Buscar customer
        var customer = await _repository.GetByIdAsync(request.Id);
        if (customer == null)
            throw new NotFoundException($"Customer com ID {request.Id} não foi encontrado");

        // Atualizar
        customer.Name = request.Name;
        customer.Email = request.Email;
        customer.Phone = request.Phone;
        customer.BirthDate = request.BirthDate;

        await _repository.UpdateAsync(customer);

        return new UpdateCustomerResponse
        {
            Id = customer.Id,
            Message = "Customer atualizado com sucesso"
        };
    }

    private async Task<bool> ValidationAsync(UpdateCustomerRequest request)
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
