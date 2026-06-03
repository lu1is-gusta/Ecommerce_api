using EcommerceApi.Application.UseCases.CancelOrder;
using EcommerceApi.Application.UseCases.CreateBuyer;
using EcommerceApi.Application.UseCases.CreateOrder;
using EcommerceApi.Application.UseCases.CreateProduct;
using EcommerceApi.Application.UseCases.DeleteBuyer;
using EcommerceApi.Application.UseCases.DeleteOrder;
using EcommerceApi.Application.UseCases.DeleteProduct;
using EcommerceApi.Application.UseCases.GetBuyers;
using EcommerceApi.Application.UseCases.GetOrders;
using EcommerceApi.Application.UseCases.GetProducts;
using EcommerceApi.Application.UseCases.UpdateBuyer;
using EcommerceApi.Application.UseCases.UpdateOrder;
using EcommerceApi.Application.UseCases.UpdateProduct;
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

        services.AddScoped<CreateProductUseCase>();
        services.AddScoped<UpdateProductUseCase>();
        services.AddScoped<DeleteProductUseCase>();
        services.AddScoped<GetProductsUseCase>();
        services.AddScoped<GetProductByIdUseCase>();

        services.AddScoped<IValidator<CreateProductRequest>, CreateProductValidator>();
        services.AddScoped<IValidator<UpdateProductRequest>, UpdateProductValidator>();

        services.AddScoped<CreateBuyerUseCase>();
        services.AddScoped<UpdateBuyerUseCase>();
        services.AddScoped<DeleteBuyerUseCase>();
        services.AddScoped<GetBuyersUseCase>();
        services.AddScoped<GetBuyerByIdUseCase>();

        services.AddScoped<IValidator<CreateBuyerRequest>, CreateBuyerValidator>();
        services.AddScoped<IValidator<UpdateBuyerRequest>, UpdateBuyerValidator>();

        return services;
    }
}
