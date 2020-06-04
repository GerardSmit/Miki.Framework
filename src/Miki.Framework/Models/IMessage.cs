namespace Miki.Framework.Models
{
    using System.Threading.Tasks;

    public interface IUser
    {	    
	    string Id { get; }

	    IPlatform Platform { get; }
	    
	    bool IsBot { get; }
	    
	    bool IsSelf { get; }
    }

    public interface IGuildUser : IUser
    {
	    string GuildId { get; }
    }
    
    public interface IMessage
	{
		string Content { get; }
		
		IPlatform Platform { get; }
		
		string ChannelId { get; }
		
		string GuildId { get; }

		Task DeleteAsync();

		Task<IChannel> GetChannelAsync();

		Task<IUser> GetAuthorAsync();

		Task<IMessage> ModifyAsync(string content);
	}
}
