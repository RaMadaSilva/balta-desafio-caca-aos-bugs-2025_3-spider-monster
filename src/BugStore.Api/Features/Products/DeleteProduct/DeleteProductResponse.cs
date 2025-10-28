namespace BugStore.Api.Features.Products.DeleteProduct; 

public class DeleteProductResponse
{
    public Guid Id { get; set; }
    public string Message { get; set; } = "Product deleted successfully";
}

