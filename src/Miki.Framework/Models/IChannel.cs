namespace Miki.Framework.Models
{
    using System.Threading.Tasks;

    public interface IChannel
    {
        string Id { get; }
        
        Task<IMessage> CreateMessageAsync(string content);
    }
}
