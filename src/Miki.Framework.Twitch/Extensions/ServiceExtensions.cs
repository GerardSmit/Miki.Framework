using Microsoft.Extensions.DependencyInjection;
using Miki.Cache;
using Miki.Framework.Twitch.Hosting;
using TwitchLib.Client.Models;

namespace Miki.Framework.Twitch.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddTwitch(this IServiceCollection services, string channel, string username, string token)
        {
            services.AddHostedService(provider =>
            {
                var credentials = new ConnectionCredentials(username, token);
                
                return new TwitchBotHostedService(credentials, channel, provider);
            });
            
            return services;
        }
    }
}