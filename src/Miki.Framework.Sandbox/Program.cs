using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Miki.Cache;
using Miki.Cache.InMemory;
using Miki.Framework.Commands;
using Miki.Framework.Commands.Filters;
using Miki.Framework.Commands.Prefixes;
using Miki.Framework.Commands.Stages;
using Miki.Framework.Discord;
using Miki.Serialization;
using Miki.Serialization.MsgPack;

namespace Miki.Framework.Sandbox
{
    internal static class Program
    {
        public static Task Main(string[] args)
        {
            return new HostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddDiscord(Environment.GetEnvironmentVariable("DISCORD_TOKEN"));
                    services.AddSingleton<ISerializer, MsgPackSerializer>();
                    services.AddSingleton<IExtendedCacheClient, InMemoryCacheClient>();

                    services.AddPrefix(builder =>
                    {
                        builder
                            .AddMention(isDefault: true)
                            .Add("miki.");
                    });
                })
                .ConfigureBot(app =>
                {
                    app.UsePrefix();
                    app.UseFilters<BotFilter>();
                    
                    app.Run(async context =>
                    {
                        var channel = await context.Message.GetChannelAsync();

                        await channel.CreateMessageAsync(context.GetQuery());
                    });
                })
                .RunConsoleAsync();
        }
    }
}