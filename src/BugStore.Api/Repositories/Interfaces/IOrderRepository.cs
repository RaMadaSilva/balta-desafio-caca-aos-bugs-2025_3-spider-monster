using BugStore.Models;

namespace BugStore.Api.Repositories.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id);
    Task<Order> CreateAsync(Order order);
}

