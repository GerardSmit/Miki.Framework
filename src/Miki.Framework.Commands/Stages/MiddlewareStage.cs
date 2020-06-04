using Miki.Framework.Models;

namespace Miki.Framework.Commands.Stages
{
    using Miki.Framework.Commands.Pipelines;
    using System;
    using System.Threading.Tasks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'MiddlewareStage'
    public class MiddlewareStage : IPipelineStage
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'MiddlewareStage'
	{
		private readonly Func<IMessage, IContext, Func<ValueTask>, ValueTask> fn;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'MiddlewareStage.MiddlewareStage(Func<IDiscordMessage, IContext, Func<ValueTask>, ValueTask>)'
		public MiddlewareStage(Func<IMessage, IContext, Func<ValueTask>, ValueTask> fn)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'MiddlewareStage.MiddlewareStage(Func<IDiscordMessage, IContext, Func<ValueTask>, ValueTask>)'
		{
			this.fn = fn;
		}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'MiddlewareStage.CheckAsync(IDiscordMessage, IContext, Func<ValueTask>)'
		public ValueTask CheckAsync(IMessage data, IContext e, Func<ValueTask> next)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'MiddlewareStage.CheckAsync(IDiscordMessage, IContext, Func<ValueTask>)'
		{
			if(fn != null)
			{
				return fn(data, e, next);
			}
            return default;
		}
	}
}

namespace Miki.Framework
{
    using System;
    using System.Threading.Tasks;
    using Miki.Framework.Commands;
    using Miki.Framework.Commands.Stages;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ContextExtensions'
    public static class ContextExtensions
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ContextExtensions'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ContextExtensions.UseStage(CommandPipelineBuilder, Func<IDiscordMessage, IContext, Func<ValueTask>, ValueTask>)'
		public static CommandPipelineBuilder UseStage(this CommandPipelineBuilder b,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ContextExtensions.UseStage(CommandPipelineBuilder, Func<IDiscordMessage, IContext, Func<ValueTask>, ValueTask>)'
			Func<IMessage, IContext, Func<ValueTask>, ValueTask> fn)
		{
			return b.UseStage(new MiddlewareStage(fn));
		}
	}
}