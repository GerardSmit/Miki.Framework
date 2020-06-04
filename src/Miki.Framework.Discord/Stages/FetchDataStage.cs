using Miki.Discord.Common;
using Miki.Framework.Models;

namespace Miki.Framework.Commands.Stages
{
    using System;
    using System.Threading.Tasks;
    using Miki.Framework.Commands.Pipelines;

    public class FetchDataStage : IPipelineStage
    {
        public static string ChannelArgumentKey = "framework-channel";
        public static string GuildArgumentKey = "framework-guild";

        public async ValueTask CheckAsync(IMessage data, IContext e, Func<ValueTask> next)
        {
            var channel = await e.Message.GetChannelAsync();
            if(channel == null)
            {
                throw new InvalidOperationException("This channel is not supported");
            }
            e.SetContext(ChannelArgumentKey, channel);
            if(channel is IDiscordGuildChannel gc)
            {
                e.SetContext(GuildArgumentKey, await gc.GetGuildAsync());
            }
            await next();
        }
    }
}

namespace Miki.Framework
{
    using Miki.Framework.Commands.Stages;

    public static class FetchDataStageExtensions
    {
        public static IDiscordTextChannel GetChannel(this IContext context)
        {
            return context.GetContext<IDiscordTextChannel>(FetchDataStage.ChannelArgumentKey);
        }

        public static IDiscordGuild GetGuild(this IContext context)
        {
            return context.GetContext<IDiscordGuild>(FetchDataStage.GuildArgumentKey);
        }
    }
}
