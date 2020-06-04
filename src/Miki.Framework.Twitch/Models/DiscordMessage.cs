using System;
using System.Threading.Tasks;
using Miki.Framework.Models;
using TwitchLib.Client.Models;

namespace Miki.Framework.Twitch.Models
{
    public class DiscordMessage : IMessage
    {
        private readonly string authorName;
        private readonly TwitchPlatform platform;
        private DiscordUser author;
        private DiscordChannel channel;

        public DiscordMessage(TwitchPlatform platform, string content, string channelId, string guildId, string authorName)
        {
            this.platform = platform;
            Content = content;
            ChannelId = channelId;
            GuildId = guildId;
            this.authorName = authorName;
        }

        public string Content { get; }

        public IPlatform Platform => platform;

        public string ChannelId { get; }

        public string GuildId { get; }

        public Task DeleteAsync()
        {
            throw new NotSupportedException();
        }

        public Task<IChannel> GetChannelAsync()
        {
            return Task.FromResult<IChannel>(channel ??= new DiscordChannel(ChannelId, platform));
        }

        public Task<IUser> GetAuthorAsync()
        {
            return Task.FromResult<IUser>(author ??= new DiscordUser(authorName, platform));
        }

        public Task<IMessage> ModifyAsync(string content)
        {
            throw new NotImplementedException();
        }
    }
}