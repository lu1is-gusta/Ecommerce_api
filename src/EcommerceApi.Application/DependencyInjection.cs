using EcommerceApi.Application.UseCases.CancelOrder;
using EcommerceApi.Application.UseCases.CreateOrder;
using EcommerceApi.Application.UseCases.DeleteOrder;
using EcommerceApi.Application.UseCases.GetOrders;
using EcommerceApi.Application.UseCases.UpdateOrder;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateOrderUseCase>();
        services.AddScoped<UpdateOrderUseCase>();
        services.AddScoped<CancelOrderUseCase>();
        services.AddScoped<DeleteOrderUseCase>();
        services.AddScoped<GetOrdersUseCase>();
        services.AddScoped<GetOrderByIdUseCase>();

        services.AddScoped<IValidator<CreateOrderRequest>, CreateOrderValidator>();
        services.AddScoped<IValidator<UpdateOrderRequest>, UpdateOrderValidator>();

        return services;
    }
}
