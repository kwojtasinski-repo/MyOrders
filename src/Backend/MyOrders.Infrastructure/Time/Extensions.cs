﻿using Microsoft.Extensions.DependencyInjection;
using MyOrders.Core.Services;

namespace MyOrders.Infrastructure.Time
{
    internal static class Extensions
    {
        public static IServiceCollection AddTime(this IServiceCollection services)
        {
            services.AddSingleton<IClock, Clock>();
            return services;
        }
    }
}
