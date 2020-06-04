using System;
using System.Threading.Tasks;
using Miki.Discord.Common;
using Miki.Framework.Models;

namespace Miki.Framework.Discord.Models
{
    public class DiscordChannel : IChannel
    {
        private readonly DiscordPlatform platform;
        private readonly IDiscordChannel discordChannel;

        public DiscordChannel(IDiscordChannel discordChannel, DiscordPlatform platform)
        {
            this.discordChannel = discordChannel;
            this.platform = platform;
        }

        public string Id => discordChannel.Id.ToString();

        public async Task<IMessage> CreateMessageAsync(string content)
        {
            return new DiscordMessage(await platform.Client.SendMessageAsync(discordChannel.Id, content), platform);
        }
    }
}