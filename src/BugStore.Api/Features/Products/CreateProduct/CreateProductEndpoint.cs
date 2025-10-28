namespace BugStore.Api.Features.Products.CreateProduct
{
    public static class CreateProductEndpoint
    {
        public static void MapCreateProductEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("/v1/products", HandleAsync)
                .WithTags("Products")
                .WithName("CreateProduct")
                .Produces<CreateProductResponse>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);
        }

        static async Task<IResult> HandleAsync(
            CreateProductRequest request,
            CreateProductHandler handler)
        {
            try
            {
                var result = await handler.HandleAsync(request);
                return Results.Created($"/v1/products/{result.Id}", result);
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

