namespace BugStore.Api.Features.Customers.GetByIdCustomer;

public static class GetByIdCustomerEndpoint
{
    public static void MapGetByIdCustomerEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/v1/customers/{id}", HandleAsync)
            .WithTags("Customers")
            .WithName("GetByIdCustomer")
            .Produces<GetByIdCustomerResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }

    static async Task<IResult> HandleAsync(
        Guid id,
        GetByIdCustomerHandler handler)
    {
        var query = new GetByIdCustomerQuery { Id = id };
        var result = await handler.HandleAsync(query);
        
        if (result == null) 
            return Results.NotFound(new { message = "Customer não encontrado" });
        
        return Results.Ok(result);
    }
}