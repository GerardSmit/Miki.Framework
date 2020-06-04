using Miki.Framework.Hosting;

namespace Miki.Framework.Commands
{
    using Miki.Framework.Commands.Prefixes;
    using Microsoft.Extensions.DependencyInjection;

    public static class PipelineBuilderExtensions
    {
        public const string PrefixMatchKey = "prefix-match";

        public static IBotApplicationBuilder UsePrefixes(
            this IBotApplicationBuilder builder)
        {
            return builder.Use<PipelineStageTrigger>();
        }

        public static string GetPrefixMatch(this IContext e)
        {
            return e.GetContext<string>(PrefixMatchKey);
        }
    }
}
