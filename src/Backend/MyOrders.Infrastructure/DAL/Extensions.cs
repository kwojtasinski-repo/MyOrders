﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyOrders.Core.Repositories;
using MyOrders.Infrastructure.DAL.Initlializer;
using MyOrders.Infrastructure.DAL.Repositories;
using MyOrders.Infrastructure.DAL.Repositories.InMemory;

namespace MyOrders.Infrastructure.DAL
{
    internal static class Extensions
    {
        public static IServiceCollection AddMySqlOptions(this IServiceCollection services)
        {
            var options = services.GetOptions<MySqlOptions>("mysql");

            services.Configure<MySqlOptions>(config =>
            {
                config.ConnectionString = options.ConnectionString;
            });

            return services;
        }

        public static IServiceCollection AddMySql<T>(this IServiceCollection services)
            where T : DbContext
        {
            var options = services.GetOptions<MySqlOptions>("mysql");
            ServerVersion serverVersion = ServerVersion.AutoDetect(options.ConnectionString);
            services.AddDbContext<T>(context => context.UseMySql(options.ConnectionString, serverVersion));
            return services;
        }

        public static IServiceCollection AddDatabase(this IServiceCollection services)
        {
            var database = services.GetOptions<Database>("database");

            services.Configure<Database>(config =>
            {
                config.DatabaseKind = database.DatabaseKind;
            });

            if (database.DatabaseKind == DatabaseKind.MySql)
            {
                services.AddMySqlOptions();
                services.AddMySql<MyOrdersDbContext>();
            }

            return services;
        }

        public static IServiceCollection AddDatabaseInitializer(this IServiceCollection services)
        {
            services.AddHostedService<DatabaseInitializer>();
            return services;
        }

        public static IServiceCollection AddInMemoryRepositories(this IServiceCollection services)
        {
            services.AddSingleton(typeof(IInMemoryRepository<>), typeof(InMemoryRepository<>));
            services.AddSingleton<IAddressRepository, InMemoryAddressRepostiory>();
            services.AddSingleton<IContactDataRepository, InMemoryContactDataRepository>();
            services.AddSingleton<ICustomerRepository, InMemoryCustomerRepository>();
            services.AddSingleton<IOrderItemRepository, InMemoryOrderItemRepository>();
            services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
            services.AddSingleton<IProductKindRepository, InMemoryProductKindRepository>();
            services.AddSingleton<IProductRepository, InMemoryProductRepository>();
            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            var database = services.GetOptions<Database>("database");

            if (database.DatabaseKind == DatabaseKind.InMemory)
            {
                services.AddInMemoryRepositories();
                return services;
            }

            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<IContactDataRepository, ContactDataRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IProductKindRepository, ProductKindRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            return services;
        }
    }
}
