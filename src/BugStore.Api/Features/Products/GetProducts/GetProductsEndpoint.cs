namespace BugStore.Api.Features.Products.GetProducts;

public static class GetProductsEndpoint
{
    public static void MapGetProductsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/v1/products", HandleAsync)
            .WithTags("Products")
            .WithName("GetProducts")
            .Produces<List<GetProductsResponse>>(StatusCodes.Status200OK);
    }

    static async Task<IResult> HandleAsync(GetProductsHandler handler)
    {
        var query = new GetProductsQuery();
        var result = await handler.HandleAsync(query);
        return Results.Ok(result);
    }
}

