using BugStore.Api.Exceptions;

namespace BugStore.Api.Features.Orders.CreateOrder
{
    public static class CreateOrderEndpoint
    {
        public static void MapCreateOrderEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("/v1/orders", HandleAsync)
                .WithTags("Orders")
                .WithName("CreateOrder")
                .Produces<CreateOrderResponse>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);
        }

        static async Task<IResult> HandleAsync(
            CreateOrderRequest request,
            CreateOrderHandler handler)
        {
            try
            {
                var result = await handler.HandleAsync(request);
                return Results.Created($"/v1/orders/{result.Id}", result);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
            catch (NotFoundException ex)
            {
                return Results.NotFound(new { message = ex.Message });
            }
        }
    }
}

