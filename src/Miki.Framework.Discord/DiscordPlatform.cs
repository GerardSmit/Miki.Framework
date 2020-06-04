using System.Threading.Tasks;
using Miki.Discord.Common;
using Miki.Framework.Discord.Models;
using Miki.Framework.Models;

namespace Miki.Framework
{
    public class DiscordPlatform : IPlatform
    {
        public DiscordPlatform(string id, IDiscordClient client, ulong selfUserId)
        {
            Id = id;
            Client = client;
            SelfUserId = selfUserId;
        }

        public string Id { get; }

        public string Type => "discord";

        public ulong SelfUserId { get; }
        
        public IDiscordClient Client { get; }

        public ValueTask<string> GetMentionAsync(string content)
        {
            var index = content.IndexOf('>');
            
            if(index == -1)
            {
                return default;
            }

            var prefix = content;
            if(content.Length > index + 1)
            {
                prefix = content.Substring(0, index + 1);
            }

            if(!Mention.TryParse(prefix, out var mention))
            {
                return default;
            }
	        
            if(SelfUserId != mention.Id)
            {
                return default;
            }
            
            return new ValueTask<string>(prefix);
        }

        public async ValueTask<bool> IsAdministratorAsync(IGuildUser user)
        {
            if (!(user is DiscordUser discordUser) || !(discordUser.InternalUser is IDiscordGuildUser guildUser))
            {
                return false;
            }

            return await guildUser.HasPermissionsAsync(GuildPermission.Administrator);
        }
    }
}