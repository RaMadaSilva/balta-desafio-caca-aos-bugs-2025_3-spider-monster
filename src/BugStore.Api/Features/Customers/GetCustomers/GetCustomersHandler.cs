using BugStore.Api.Repositories.Interfaces;
using BugStore.Models;

namespace BugStore.Api.Features.Customers.GetCustomers;

public class GetCustomersHandler
{
    private readonly ICustomerRepository _repository;

    public GetCustomersHandler(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<GetCustomersResponse>> HandleAsync(GetCustomersQuery query)
    {
        var customers = await _repository.GetAllAsync();

        return customers.Select(c => new GetCustomersResponse
        {
            Id = c.Id,
            Name = c.Name,
            Email = c.Email,
            Phone = c.Phone,
            BirthDate = c.BirthDate
        }).ToList();
    }
}
