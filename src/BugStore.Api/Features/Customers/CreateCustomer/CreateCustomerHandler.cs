using BugStore.Api.Repositories.Interfaces;
using BugStore.Models;
using FluentValidation;

namespace BugStore.Api.Features.Customers.CreateCustomer; 

public class CreateCustomerHandler
{
    private readonly ICustomerRepository _repository;
    private readonly AbstractValidator<CreateCustomerRequest> _validator;

    public CreateCustomerHandler(ICustomerRepository repository, AbstractValidator<CreateCustomerRequest> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<CreateCustomerResponse> HandleAsync(CreateCustomerRequest request)
    {
        // Validação
        if(!await ValidationAsync(request))
            throw new ArgumentException("Invalid customer data.");

        // Criar entidade
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            Phone = request.Phone,
            BirthDate = request.BirthDate
        };

        var createdCustomer = await _repository.CreateAsync(customer);

        // Retornar resposta
        return new CreateCustomerResponse
        {
            Id = createdCustomer.Id,
            Name = createdCustomer.Name,
            Email = createdCustomer.Email,
            CreatedAt = DateTime.Now
        };
    }

    private async Task<bool> ValidationAsync(CreateCustomerRequest request)
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
