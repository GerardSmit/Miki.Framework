using System;
using Miki.Framework.Models;

namespace Miki.Framework.Twitch.Models
{
    public class DiscordUser : IUser
    {
        private readonly string username;
        private readonly TwitchPlatform platform;
        
        public DiscordUser(string username, TwitchPlatform platform)
        {
            this.username = username;
            this.platform = platform;
        }

        public string Id => username;

        public IPlatform Platform => platform;

        public bool IsBot => platform.Username.Equals(username, StringComparison.OrdinalIgnoreCase); // TODO (GerardSmit): Validate this

        public bool IsSelf => Id == platform.Username;
    }
}