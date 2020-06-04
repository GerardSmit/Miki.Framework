using System;
using Microsoft.Extensions.DependencyInjection;

namespace Miki.Framework.Commands.Prefixes
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddPrefix(this IServiceCollection services, Action<PrefixCollectionBuilder> configure)
        {
            var builder = new PrefixCollectionBuilder();
            configure(builder);
            services.AddSingleton(builder.Build());
            services.AddScoped<IPrefixService, PrefixService>();
            return services;
        }
    }
}