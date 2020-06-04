using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Miki.Discord;
using Miki.Discord.Common;
using Miki.Framework.Discord.Models;
using Miki.Framework.Hosting;

namespace Miki.Framework.Discord.Hosting
{
    public class DiscordBotHostedService : IHostedService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IDiscordClient discordClient;
        private readonly IBotApplicationBuilder botApplicationBuilder;
        private DiscordPlatform platform;
        private bool isRunning;
        private MessageDelegate pipeline;

        public DiscordBotHostedService(DiscordClientConfigurations config, IServiceProvider serviceProvider)
        {
            botApplicationBuilder = serviceProvider.GetRequiredService<IBotApplicationBuilder>();
            this.serviceProvider = serviceProvider;
            
            discordClient = new DiscordClient(config);
        }

        /// <summary>
        /// Handles incoming messages from <see cref="discordClient"/>.
        /// </summary>
        /// <param name="data">Incoming message.</param>
        private async Task HandleMessageAsync(IDiscordMessage data)
        {
            var scope = serviceProvider.CreateScope();
            var message = new DiscordMessage(data, platform);
            
            await using ((IAsyncDisposable)scope)
            {
                using var context = new ContextObject(scope.ServiceProvider, message);
            
                context.SetQuery(data.Content); // TODO (GerardSmit): Is this da wae?
            
                await pipeline(context);
            }
        }

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (isRunning)
            {
                throw new InvalidOperationException($"{nameof(DiscordBotHostedService)} is already running");
            }
            
            isRunning = true;
            
            await discordClient.Gateway.StartAsync();

            var id = (await discordClient.GetSelfAsync()).Id;
            
            platform = new DiscordPlatform($"discord:{id}", discordClient, id);
            pipeline = botApplicationBuilder.Build();
            discordClient.MessageCreate += HandleMessageAsync;
        }

        /// <inheritdoc />
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            discordClient.MessageCreate -= HandleMessageAsync;
            await discordClient.Gateway.StopAsync();
            isRunning = false;
        }
    }
}