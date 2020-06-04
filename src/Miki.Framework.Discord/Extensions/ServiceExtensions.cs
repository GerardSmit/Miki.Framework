using Microsoft.Extensions.DependencyInjection;
using Miki.Cache;
using Miki.Discord;
using Miki.Discord.Gateway;
using Miki.Discord.Rest;
using Miki.Framework.Discord.Hosting;

namespace Miki.Framework.Discord
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddDiscord(this IServiceCollection services, string token)
        {
            services.AddHostedService(provider =>
            {
                var cacheClient = provider.GetRequiredService<IExtendedCacheClient>();
                var config = new DiscordClientConfigurations
                {
                    Gateway = new GatewayCluster(
                        new GatewayProperties
                        {
                            Token = token
                        }
                    ),
                    ApiClient = new DiscordApiClient(token, cacheClient),
                    CacheClient = cacheClient
                };
                
                return new DiscordBotHostedService(config, provider);
            });
            
            return services;
        }
    }
}