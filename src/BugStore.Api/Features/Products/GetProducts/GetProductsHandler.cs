using BugStore.Api.Repositories.Interfaces;

namespace BugStore.Api.Features.Products.GetProducts;

public class GetProductsHandler
{
    private readonly IProductRepository _repository;

    public GetProductsHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<GetProductsResponse>> HandleAsync(GetProductsQuery query)
    {
        var products = await _repository.GetAllAsync();

        return products.Select(p => new GetProductsResponse
        {
            Id = p.Id,
            Title = p.Title,
            Description = p.Description,
            Slug = p.Slug,
            Price = p.Price
        }).ToList();
    }
}
