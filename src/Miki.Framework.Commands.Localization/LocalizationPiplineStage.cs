using Miki.Framework.Models;

namespace Miki.Framework.Commands.Localization
{
    using Miki.Framework.Commands.Pipelines;
    using Miki.Localization;
    using System;
    using System.Threading.Tasks;
    using Miki.Framework.Commands.Localization.Services;

    public class LocalizationPipelineStage : IPipelineStage
    {
        public const string LocaleContextKey = "framework-localization";

        private readonly ILocalizationService service;

        public LocalizationPipelineStage(ILocalizationService service)
        {
            this.service = service;
        }

        public async ValueTask CheckAsync(IMessage data, IContext e, Func<ValueTask> next)
        {
            var channel = await e.Message.GetChannelAsync();
            
            // TODO (GerardSmit): In Twitch the ID is the channel name. The ILocalizationService should accept a string instead of a long.
            if (long.TryParse(channel.Id, out var id))
            {
                var locale = await service.GetLocaleAsync(id);
                e.SetContext(LocaleContextKey, locale);
            }
            else
            {
                // TODO: add GetDefaultLocale to ILocalizationService.
                if(!(service is LocalizationService extService))
                {
                    throw new NotSupportedException("Cannot fetch default locale from service");
                }

                var locale = extService.GetDefaultLocale();
                e.SetContext(LocaleContextKey, locale);
            }

            await next();
        }
    }
}