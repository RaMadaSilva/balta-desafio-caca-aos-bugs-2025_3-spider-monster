using BugStore.Api.Features.Customers.CreateCustomer;
using BugStore.Api.Features.Customers.DeleteCustomer;
using BugStore.Api.Features.Customers.GetByIdCustomer;
using BugStore.Api.Features.Customers.GetCustomers;
using BugStore.Api.Features.Customers.UpdateCustomer;
using BugStore.Api.Features.Orders.CreateOrder;
using BugStore.Api.Features.Orders.GetOrderById;
using BugStore.Api.Features.Products.CreateProduct;
using BugStore.Api.Features.Products.DeleteProduct;
using BugStore.Api.Features.Products.GetProductById;
using BugStore.Api.Features.Products.GetProducts;
using BugStore.Api.Features.Products.UpdateProduct;

namespace BugStore.Api.ExtensionsMethods; 

public static class Endpoints
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapGet("/", () => "BugStore API - Hello World!");

        // Registrar Endpoints - Customers
        app.MapCreateCustomerEndpoint();
        app.MapUpdateCustomerEndpoint();
        app.MapDeleteCustomerEndpoint();
        app.MapGetCustomersEndpoint();
        app.MapGetByIdCustomerEndpoint();

        // Registrar Endpoints - Products
        app.MapCreateProductEndpoint();
        app.MapUpdateProductEndpoint();
        app.MapDeleteProductEndpoint();
        app.MapGetProductsEndpoint();
        app.MapGetProductByIdEndpoint();

        // Registrar Endpoints - Orders
        app.MapCreateOrderEndpoint();
        app.MapGetOrderByIdEndpoint();
    }
}
