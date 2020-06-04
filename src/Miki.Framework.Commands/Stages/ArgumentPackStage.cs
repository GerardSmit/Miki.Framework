using Miki.Framework.Hosting;
using Miki.Framework.Models;

namespace Miki.Framework.Commands.Pipelines
{
    using Miki.Framework.Arguments;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Allows you to use FIFO-like argument readers. Used in other packages as a dependency.
    /// </summary>
	public class ArgumentPackBuilder : IPipelineStage
	{
		internal static string ArgumentKey = "framework-arguments";

		private readonly ArgumentParseProvider provider;

        /// <summary>
        /// Allows you to use FIFO-like argument readers. Used in other packages as a dependency.
        /// </summary>
		public ArgumentPackBuilder()
            : this(new ArgumentParseProvider())
		{ }

        /// <summary>
        /// Allows you to use FIFO-like argument readers. Used in other packages as a dependency. With 
        /// non-default providers if you prefer overriding default implementation.
        /// </summary>
		public ArgumentPackBuilder(ArgumentParseProvider provider)
		{
			this.provider = provider;
		}

		/// <inheritdoc/>
		public async ValueTask CheckAsync(IMessage message, IContext e, Func<ValueTask> next)
		{
			e.SetContext<ITypedArgumentPack>(
				ArgumentKey,
				new TypedArgumentPack(
					new ArgumentPack(
                        e.GetQuery().Split(' ').Where(x => !string.IsNullOrWhiteSpace(x))), provider));
			await next();
		}
	}
}

namespace Miki.Framework.Commands
{
    using Miki.Framework.Arguments;
    using Miki.Framework.Commands.Pipelines;

	/// <summary>
	/// Helper class for builder extensions
	/// </summary>
    public static class ArgumentPackCommandPipelineExtensions
	{
		/// <summary>
		/// Allows you to use FIFO-like argument readers. Used in other packages as a dependency.
		/// </summary>
		public static IBotApplicationBuilder UseArgumentPack(this IBotApplicationBuilder builder)
		{
			return builder.Use<ArgumentPackBuilder>();
		}
		
        /// <summary>
        /// Allows you to use FIFO-like argument readers. Used in other packages as a dependency. With 
        /// non-default providers if you prefer overriding default implementation.
        /// </summary>
		public static IBotApplicationBuilder UseArgumentPack(
           this IBotApplicationBuilder builder, ArgumentParseProvider provider)
		{
			return builder.Use(new ArgumentPackBuilder(provider));
		}
	}
}

namespace Miki.Framework
{
    using Miki.Framework.Arguments;
    using Miki.Framework.Commands.Pipelines;

    /// <summary>
    /// Helper class for context extensions
    /// </summary>
	public static class ArgumentPackContextExtensions
	{
		/// <summary>
		/// Gets this context's arguments.
		/// </summary>
		public static ITypedArgumentPack GetArgumentPack(this IContext context)
		{
			return context.GetContext<ITypedArgumentPack>(ArgumentPackBuilder.ArgumentKey);
		}
	}
}