using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Miki.Framework.Hosting;
using Miki.Framework.Twitch.Models;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace Miki.Framework.Twitch.Hosting
{
    public class TwitchBotHostedService : IHostedService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ITwitchClient twitchClient;
        private readonly IBotApplicationBuilder botApplicationBuilder;
        private TwitchPlatform platform;
        private bool isRunning;
        private MessageDelegate pipeline;
        private readonly ConnectionCredentials credentials;
        private readonly string channel;

        public TwitchBotHostedService(ConnectionCredentials credentials, string channel,
            IServiceProvider serviceProvider)
        {
            botApplicationBuilder = serviceProvider.GetRequiredService<IBotApplicationBuilder>();
            this.serviceProvider = serviceProvider;
            this.credentials = credentials;
            this.channel = channel;
            
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
            WebSocketClient customClient = new WebSocketClient(clientOptions);

            twitchClient = new TwitchClient(customClient);
            twitchClient.Initialize(credentials, channel);
        }

        /// <summary>
        /// Handles incoming messages from <see cref="twitchClient"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Incoming message.</param>
        private void HandleMessage(object sender, OnMessageReceivedArgs e)
        {
            _ = Task.Run(async () =>
            {
                var scope = serviceProvider.CreateScope();
                var message = new DiscordMessage(
                    platform, 
                    e.ChatMessage.Message,
                    e.ChatMessage.Channel,
                    e.ChatMessage.Channel,
                    e.ChatMessage.Username);

                await using ((IAsyncDisposable) scope)
                {
                    using var context = new ContextObject(scope.ServiceProvider, message);

                    context.SetQuery(e.ChatMessage.Message); // TODO (GerardSmit): Is this da wae?

                    await pipeline(context);
                }
            });
        }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (isRunning)
            {
                throw new InvalidOperationException($"{nameof(TwitchBotHostedService)} is already running");
            }
            
            isRunning = true;
            platform = new TwitchPlatform($"twitch:{credentials.TwitchUsername}", twitchClient, credentials.TwitchUsername);
            pipeline = botApplicationBuilder.Build();
            twitchClient.OnMessageReceived += HandleMessage;

            twitchClient.Connect();
            
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            twitchClient.OnMessageReceived -= HandleMessage;
            twitchClient.Disconnect();
            isRunning = false;
            
            return Task.CompletedTask;
        }
    }
}