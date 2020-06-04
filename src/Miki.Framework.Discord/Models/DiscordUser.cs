using Miki.Discord.Common;
using Miki.Framework.Models;

namespace Miki.Framework.Discord.Models
{
    public class DiscordUser : IUser
    {
        private readonly DiscordPlatform platform;
        
        public DiscordUser(IDiscordUser internalUser, DiscordPlatform platform)
        {
            InternalUser = internalUser;
            this.platform = platform;
        }
        
        internal IDiscordUser InternalUser { get; }

        public string Id => InternalUser.Id.ToString();

        public IPlatform Platform => platform;

        public bool IsBot => InternalUser.IsBot;

        public bool IsSelf => InternalUser.Id == platform.SelfUserId;
    }
}