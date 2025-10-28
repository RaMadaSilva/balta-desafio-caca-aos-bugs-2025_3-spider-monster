namespace BugStore.Api.Features.Customers.CreateCustomer
{
    public static class CreateCustomerEndpoint
    {
        public static void MapCreateCustomerEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("/v1/customers", HandleAsync)
                .WithTags("Customers")
                .WithName("CreateCustomer")
                .Produces<CreateCustomerResponse>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);
        }

        static async Task<IResult> HandleAsync(
            CreateCustomerRequest request,
            CreateCustomerHandler handler)
        {
            try
            {
                var result = await handler.HandleAsync(request);
                return Results.Created($"/v1/customers/{result.Id}", result);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        }
    }
}
