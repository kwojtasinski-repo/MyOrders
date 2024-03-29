﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyOrders.Infrastructure.App;
using MyOrders.Infrastructure.Conventions;
using MyOrders.Infrastructure.DAL;
using MyOrders.Infrastructure.Exceptions;
using MyOrders.Infrastructure.Time;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("MyOrders.IntegrationTests")]
namespace MyOrders.Infrastructure
{
    public static class Extensions
    {
        private const string Cors = "Cors";

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AppOptions>(configuration.GetRequiredSection("app"));
            services.AddControllers(options => options.UseDashedConventionInRouting());
            services.AddTime();
            services.AddDatabase();
            services.AddRepositories();
            services.AddDatabaseInitializer();
            services.AddExceptionHandling();
            services.AddSwaggerGen();
            services.AddCors(options =>
                    options.AddPolicy(Cors, policy =>
                                policy.AllowAnyMethod()
                                      .AllowAnyHeader()
                                      .AllowAnyOrigin()));
            return services;
        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
        {
            app.UseExceptionHandling();
            app.UseCors(Cors);
            app.UseSwagger();
            app.UseSwaggerUI();
            return app;
        }

        public static T GetOptions<T>(this IServiceCollection services, string sectionName) where T : new()
        {
            using var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            return configuration.GetOptions<T>(sectionName);
        }

        public static T GetOptions<T>(this IConfiguration configuration, string sectionName) where T : new()
        {
            var options = new T();
            configuration.GetSection(sectionName).Bind(options);
            return options;
        }
    }
}