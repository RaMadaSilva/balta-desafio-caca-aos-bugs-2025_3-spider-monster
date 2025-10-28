using BugStore.Api.Repositories.Interfaces;

namespace BugStore.Api.Features.Customers.GetByIdCustomer;

public class GetByIdCustomerHandler
{
    private readonly ICustomerRepository _repository;

    public GetByIdCustomerHandler(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetByIdCustomerResponse?> HandleAsync(GetByIdCustomerQuery query)
    {
        var customer = await _repository.GetByIdAsync(query.Id);

        if (customer == null) 
            return null;

        return new GetByIdCustomerResponse
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            Phone = customer.Phone,
            BirthDate = customer.BirthDate
        };
    }
}
