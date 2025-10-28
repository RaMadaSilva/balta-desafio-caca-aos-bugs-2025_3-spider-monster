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
using BugStore.Api.Repositories;
using BugStore.Api.Repositories.Interfaces;
using BugStore.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BugStore.Api.ExtensionsMethods
{
    public static class Registers
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection"); 

            services.AddDbContext<AppDbContext>(x => x.UseSqlite(connectionString));
            return services;

        }

        public static IServiceCollection AddDependecyInjection(this IServiceCollection services)
        {
            // Registrar Repositories (Abstrações)
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();

            // Registrar Validators
            services.AddScoped<AbstractValidator<CreateCustomerRequest>, CreateCustomerValidator>();
            services.AddScoped<AbstractValidator<UpdateCustomerRequest>, UpdateCustomerValidator>();
            services.AddScoped<AbstractValidator<DeleteCustomerRequest>, DeleteCustomerValidator>();
            services.AddScoped<AbstractValidator<CreateProductRequest>, CreateProductValidator>();
            services.AddScoped<AbstractValidator<UpdateProductRequest>, UpdateProductValidator>();
            services.AddScoped<AbstractValidator<DeleteProductRequest>, DeleteProductValidator>();
            services.AddScoped<AbstractValidator<CreateOrderRequest>, CreateOrderValidator>();

            // Registrar Handlers - Customers
            services.AddScoped<CreateCustomerHandler>();
            services.AddScoped<UpdateCustomerHandler>();
            services.AddScoped<DeleteCustomerHandler>();
            services.AddScoped<GetCustomersHandler>();
            services.AddScoped<GetByIdCustomerHandler>();

            // Registrar Handlers - Products
            services.AddScoped<CreateProductHandler>();
            services.AddScoped<UpdateProductHandler>();
            services.AddScoped<DeleteProductHandler>();
            services.AddScoped<GetProductsHandler>();
            services.AddScoped<GetProductByIdHandler>();

            // Registrar Handlers - Orders
            services.AddScoped<CreateOrderHandler>();
            services.AddScoped<GetOrderByIdHandler>();

            return services;
        }
    }
}