using System.Threading.Tasks;
using Miki.Framework.Models;

namespace Miki.Framework
{
    public interface IPlatform
    {
        string Id { get; }
        
        string Type { get; }
        
        ValueTask<string> GetMentionAsync(string content);
        
        ValueTask<bool> IsAdministratorAsync(IGuildUser user);
    }
}