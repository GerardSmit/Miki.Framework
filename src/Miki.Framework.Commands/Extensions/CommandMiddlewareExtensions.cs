using System;
using Microsoft.Extensions.DependencyInjection;
using Miki.Framework.Commands.Pipelines;
using Miki.Framework.Hosting;
using Miki.Logging;

namespace Miki.Framework.Commands
{
    public static class CommandMiddlewareExtensions
    {
        public static IBotApplicationBuilder Use(this IBotApplicationBuilder app, IPipelineStage stage)
        {
            return app.Use(next =>
            {
                return context => stage.CheckAsync(context.Message, context, () => next(context));
            });
        }

        public static IBotApplicationBuilder Use<T>(this IBotApplicationBuilder app)
            where T : IPipelineStage
        {
            var stage = app.ApplicationServices.GetOrCreateService<T>();
            
            return Use(app, stage);
        }
    }
}