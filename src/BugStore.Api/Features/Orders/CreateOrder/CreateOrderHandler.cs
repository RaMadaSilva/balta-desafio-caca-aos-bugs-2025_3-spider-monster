using BugStore.Api.Exceptions;
using BugStore.Api.Repositories.Interfaces;
using BugStore.Models;
using FluentValidation;

namespace BugStore.Api.Features.Orders.CreateOrder; 

public class CreateOrderHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;
    private readonly AbstractValidator<CreateOrderRequest> _validator;

    public CreateOrderHandler(
        IOrderRepository orderRepository,
        ICustomerRepository customerRepository,
        IProductRepository productRepository,
        AbstractValidator<CreateOrderRequest> validator)
    {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _productRepository = productRepository;
        _validator = validator;
    }

    public async Task<CreateOrderResponse> HandleAsync(CreateOrderRequest request)
    {
        // Validação
        if(!await ValidationAsync(request))
            throw new ArgumentException("Invalid order data.");

        // Validar se o customer existe
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
        if (customer == null)
            throw new NotFoundException($"Customer com ID {request.CustomerId} não foi encontrado");

        // Validar produtos e coletar dados
        var productIds = request.Items.Select(i => i.ProductId).ToList();
        var products = (await _productRepository.GetByIdsAsync(productIds)).ToList();

        if (products.Count != request.Items.Count)
            throw new InvalidOperationException("Alguns produtos não foram encontrados");

        // Criar ordem
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            Lines = new List<OrderLine>()
        };

        decimal total = 0;
        var orderLines = new List<OrderLineResponse>();

        // Criar linhas da ordem
        foreach (var item in request.Items)
        {
            var product = products.First(p => p.Id == item.ProductId);
            var lineTotal = product.Price * item.Quantity;
            total += lineTotal;

            var orderLine = new OrderLine
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Total = lineTotal
            };

            order.Lines.Add(orderLine);

            orderLines.Add(new OrderLineResponse
            {
                ProductId = product.Id,
                ProductTitle = product.Title,
                Quantity = item.Quantity,
                Price = product.Price,
                Total = lineTotal
            });
        }

        await _orderRepository.CreateAsync(order);

        // Retornar resposta
        return new CreateOrderResponse
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            Total = total,
            Items = orderLines,
            CreatedAt = order.CreatedAt
        };
    }

    private async Task<bool> ValidationAsync(CreateOrderRequest request)
    {
        var result = await _validator.ValidateAsync(request);
        
        if(!result.IsValid)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.ErrorMessage));
            return false; 
        }

        return true;
    }
}
