using System.Threading.Tasks;
using Miki.Framework.Models;

namespace Miki.Framework.Twitch.Models
{
    public class DiscordChannel : IChannel
    {
        private readonly TwitchPlatform platform;

        public DiscordChannel(string channel, TwitchPlatform platform)
        {
            Id = channel;
            this.platform = platform;
        }

        public string Id { get; }

        public Task<IMessage> CreateMessageAsync(string content)
        {
            platform.Client.SendMessage(Id, content);
            
            return Task.FromResult<IMessage>(new DiscordMessage(platform, content, Id, Id, platform.Username));
        }
    }
}