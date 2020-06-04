using System;
using System.Threading.Tasks;
using Miki.Discord.Common;
using Miki.Framework.Models;

namespace Miki.Framework.Discord.Models
{
    public class DiscordMessage : IMessage
    {
        private readonly IDiscordMessage discordMessage;
        private readonly DiscordPlatform platform;
        private DiscordUser author;
        private DiscordChannel channel;

        public DiscordMessage(IDiscordMessage discordMessage, DiscordPlatform platform)
        {
            this.discordMessage = discordMessage;
            this.platform = platform;
        }

        public string Content => discordMessage.Content;

        public IPlatform Platform => platform;

        public string ChannelId => discordMessage.ChannelId.ToString();

        public string GuildId => (discordMessage.Author as IDiscordGuildUser)?.Id.ToString();

        public Task DeleteAsync()
        {
            return discordMessage.DeleteAsync();
        }

        public async Task<IChannel> GetChannelAsync()
        {
            return channel ??= new DiscordChannel(await discordMessage.GetChannelAsync(), platform);
        }

        public Task<IUser> GetAuthorAsync()
        {
            return Task.FromResult<IUser>(author ??= new DiscordUser(discordMessage.Author, platform));
        }

        public async Task<IMessage> ModifyAsync(string content)
        {
            return new DiscordMessage(await platform.Client.EditMessageAsync(discordMessage.ChannelId, discordMessage.Id, content), platform);
        }
    }
}