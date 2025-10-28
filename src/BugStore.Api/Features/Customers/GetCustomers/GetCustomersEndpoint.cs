namespace BugStore.Api.Features.Customers.GetCustomers;

public static class GetCustomersEndpoint
{
    public static void MapGetCustomersEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/v1/customers", HandleAsync)
            .WithTags("Customers")
            .WithName("GetCustomers")
            .Produces<List<GetCustomersResponse>>(StatusCodes.Status200OK);
    }

    static async Task<IResult> HandleAsync(GetCustomersHandler handler)
    {
        var query = new GetCustomersQuery();
        var result = await handler.HandleAsync(query);
        return Results.Ok(result);
    }
}