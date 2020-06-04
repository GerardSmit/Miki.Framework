using System;

namespace Miki.Framework.Commands.Filters
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public readonly struct UserRule : IEquatable<UserRule>
    {
	    public UserRule(string platformType, string id)
	    {
		    PlatformType = platformType;
		    Id = id;
	    }

	    public string PlatformType { get; }
	    
	    public string Id { get; }

	    public bool Equals(UserRule other)
	    {
		    return PlatformType == other.PlatformType && Id == other.Id;
	    }

	    public override bool Equals(object obj)
	    {
		    return obj is UserRule other && Equals(other);
	    }

	    public override int GetHashCode()
	    {
		    return HashCode.Combine(PlatformType, Id);
	    }

	    public static bool operator ==(UserRule left, UserRule right)
	    {
		    return left.Equals(right);
	    }

	    public static bool operator !=(UserRule left, UserRule right)
	    {
		    return !left.Equals(right);
	    }
    }
    
	/// <inheritdoc/>
    public class UserFilter : IFilter
	{
		public HashSet<UserRule> Rules { get; } = new HashSet<UserRule>();

        /// <inheritdoc/>
		public async ValueTask<bool> CheckAsync(IContext msg)
        {
	        var author = await msg.Message.GetAuthorAsync();
	        
	        return !Rules.Contains(new UserRule(author.Platform.Type, author.Id));
        }
	}
}