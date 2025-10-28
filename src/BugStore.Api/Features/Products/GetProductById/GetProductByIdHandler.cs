using BugStore.Api.Repositories.Interfaces;

namespace BugStore.Api.Features.Products.GetProductById;

public class GetProductByIdHandler
{
    private readonly IProductRepository _repository;

    public GetProductByIdHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetProductByIdResponse?> HandleAsync(GetProductByIdQuery query)
    {
        var product = await _repository.GetByIdAsync(query.Id);

        if (product == null) 
            return null;

        return new GetProductByIdResponse
        {
            Id = product.Id,
            Title = product.Title,
            Description = product.Description,
            Slug = product.Slug,
            Price = product.Price
        };
    }
}
