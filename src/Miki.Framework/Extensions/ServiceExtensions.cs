using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Miki.Cache;
using Miki.Framework.Hosting;

namespace Miki.Framework
{
    public static class ServiceCollectionExtensions
    {
        public static T GetOrCreateService<T>(this IServiceProvider provider)
        {
            return provider.GetService<T>() ?? ActivatorUtilities.CreateInstance<T>(provider);
        }
        
        public static IServiceCollection AddBotApplicationBuilderFactory(this IServiceCollection services, Func<IServiceProvider, IBotApplicationBuilder> factory)
        {
            services.AddSingleton<IBotApplicationBuilderFactory>(provider => new DefaultBotApplicationBuilderFactory(factory, provider));
            return services;
        }

        private class DefaultBotApplicationBuilderFactory : IBotApplicationBuilderFactory
        {
            private readonly Func<IServiceProvider, IBotApplicationBuilder> factory;
            private readonly IServiceProvider serviceProvider;

            public DefaultBotApplicationBuilderFactory(Func<IServiceProvider, IBotApplicationBuilder> factory, IServiceProvider serviceProvider)
            {
                this.factory = factory;
                this.serviceProvider = serviceProvider;
            }

            public IBotApplicationBuilder CreateBuilder()
            {
                return factory(serviceProvider);
            }
        }
    }
}