using AccommodationService.Common.Events;
using AccommodationService.IntegrationEvents.Handlers;
using AccommodationService.Repositories;
using AccommodationService.Repositories.Interfaces;
using AccommodationService.Services;
using AccommodationService.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AccommodationService.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAccommodationServiceDependencies(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IAccommodationService, Services.AccommodationService>();
        services.AddScoped<IAvailabilityService, AvailabilityService>();
        services.AddScoped<IIntegrationEventDispatcher, IntegrationEventDispatcher>();

        services.AddScoped<IIntegrationEventHandler<UserDeletedIntegrationEvent>, UserDeletedIntegrationEventHandler>();
        services.AddScoped<IIntegrationEventHandler<ReservationCreatedIntegrationEvent>, ReservationCreatedIntegrationEventHandler>();

        services.AddScoped<IRoutedIntegrationEventHandler, RoutedHandler<UserDeletedIntegrationEvent>>();
        services.AddScoped<IRoutedIntegrationEventHandler, RoutedHandler<ReservationCreatedIntegrationEvent>>();

        services.AddHostedService<IntegrationEventsSubscriber>();

        return services;
    }

    public static IServiceCollection AddSwaggerGenWithAuth(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme,
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT"
                });

            options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            {
                {
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Reference = new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
        return services;
    }
}