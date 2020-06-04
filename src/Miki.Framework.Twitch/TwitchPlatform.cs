using System.Threading.Tasks;
using Miki.Framework.Models;
using TwitchLib.Client.Interfaces;

namespace Miki.Framework.Twitch
{
    public class TwitchPlatform : IPlatform
    {
        public TwitchPlatform(string id, ITwitchClient client, string username)
        {
            Id = id;
            Client = client;
            Username = username;
        }

        public string Id { get; }

        public string Type => "twitch";

        public string Username { get; }
        
        public ITwitchClient Client { get; }

        public ValueTask<string> GetMentionAsync(string content)
        {
            // TODO (GerardSmit): Implement this
            return new ValueTask<string>((string) null);
        }

        public ValueTask<bool> IsAdministratorAsync(IGuildUser user)
        {
            // TODO (GerardSmit): Implement this
            return new ValueTask<bool>(false);
        }
    }
}