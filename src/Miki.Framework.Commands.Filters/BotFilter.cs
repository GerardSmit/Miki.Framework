namespace Miki.Framework.Commands.Filters
{
    using System.Threading.Tasks;


    /// <inheritdoc/>
	public class BotFilter : IFilter
	{
        /// <inheritdoc/>
		public async ValueTask<bool> CheckAsync(IContext e)
        {
	        var author = await e.Message.GetAuthorAsync();
			
			return !author.IsBot;
		}
	}
}
