using Application.Interfaces;
using Azure.Messaging.ServiceBus;
using Domain.Primitives;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services;
using Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using StackExchange.Redis;

namespace Infrastructure
{
    public static class DependecyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddPersistencePostgreSQL(configuration);
            services.AddAzureServiceBus(configuration);
            services.AddRedis(configuration);
            services.AddMinio(configuration);

            return services;
        }

        private static IServiceCollection AddPersistencePostgreSQL(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(optionsBuilder =>
            {
                optionsBuilder.UseNpgsql(configuration.GetConnectionString("DatabaseConnection"),
                    options => options.EnableRetryOnFailure());
            });

            services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
            services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

            // Repositories dependency injection
            services.AddScoped(typeof(IBaseRepository<,>), typeof(BaseRepository<,>));
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IReservationRepository, ReservationRepository>();

            return services;
        }

        private static IServiceCollection AddAzureServiceBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ServiceBusClient>(options =>
            {
                var connectionString = configuration.GetConnectionString("AzureServiceBus");
                return new ServiceBusClient(connectionString);
            });

            services.AddScoped<IMessageBusService, AzureServiceBusService>();


            return services;
        }

        private static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConnectionMultiplexer>(options =>
            {
                var redisSettings = configuration.GetSection("RedisSettings").Get<RedisSettings>()
                    ?? throw new InvalidOperationException("RedisSettings configuration section is missing or invalid.");
                return ConnectionMultiplexer.Connect(redisSettings!.ConnectionString);
            });

            services.AddTransient<IRedisCacheService, RedisCacheService>();

            return services;
        }

        public static IServiceCollection AddMinio(this IServiceCollection services, IConfiguration configuration)
        {
            var minioSettings = configuration.GetSection("MinioSettings").Get<MinioSettings>() 
                ?? throw new InvalidOperationException("MinioSettings configuration section is missing or invalid.");

            services.AddSingleton(minioSettings!);

            services.AddSingleton<IMinioClient>(sp =>
            {
                return new MinioClient()
                    .WithEndpoint(minioSettings!.Endpoint)
                    .WithCredentials(minioSettings.AccessKey, minioSettings.SecretKey)
                    .Build();
            });

            // (opcional) Podrías registrar también un servicio que use MinioClient aquí
            services.AddTransient<IFileStorageService, MinioFileStorageService>();

            // (opcional) Podrías registrar un servicio para inicializar el bucket de Minio
            services.AddHostedService<MinioBucketInitializerService>();

            return services;
        }
    }
}
