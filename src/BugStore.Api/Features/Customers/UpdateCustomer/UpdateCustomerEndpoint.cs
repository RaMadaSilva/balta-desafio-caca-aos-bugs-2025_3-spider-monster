using BugStore.Api.Exceptions;

namespace BugStore.Api.Features.Customers.UpdateCustomer;

public static class UpdateCustomerEndpoint
{
    public static void MapUpdateCustomerEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/v1/customers/{id}", HandleAsync)
            .WithTags("Customers")
            .WithName("UpdateCustomer")
            .Produces<UpdateCustomerResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);
    }

    static async Task<IResult> HandleAsync(
        Guid id,
        UpdateCustomerRequest request,
        UpdateCustomerHandler handler)
    {
        try
        {
            request.Id = id;
            var result = await handler.HandleAsync(request);
            return Results.Ok(result);
        }
        catch (NotFoundException ex)
        {
            return Results.NotFound(new { message = ex.Message });
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