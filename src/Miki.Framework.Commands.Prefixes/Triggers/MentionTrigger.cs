namespace Miki.Framework.Commands.Prefixes.Triggers
{
    using System.Threading.Tasks;

    public class MentionTrigger : ITrigger
	{
		public Task<string> CheckTriggerAsync(IContext context)
        {
	        return context.Message.Platform.GetMentionAsync(context.GetQuery()).AsTask();
        }
	}
}
