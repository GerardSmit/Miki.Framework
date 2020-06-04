using System;
using Microsoft.Extensions.DependencyInjection;
using Miki.Framework.Commands.Pipelines;
using Miki.Framework.Hosting;
using Miki.Logging;

namespace Miki.Framework.Commands
{
    public static class CommandMiddlewareExtensions
    {
        /// <summary>
        /// Adds a middleware delegate defined in-line to the application's request pipeline.
        /// </summary>
        /// <param name="app">The <see cref="IBotApplicationBuilder"/> instance.</param>
        /// <param name="builder">A function that builds the pipeline.</param>
        /// <returns>The <see cref="IBotApplicationBuilder"/> instance.</returns>
        public static IBotApplicationBuilder UseCommandPipeline(this IBotApplicationBuilder app, Action<CommandPipelineBuilder> builder)
        {
            var pipelineBuilder = new CommandPipelineBuilder(app.ApplicationServices);
            
            builder(pipelineBuilder.UseStage(new CorePipelineStage()));

            var pipeline = pipelineBuilder.Build();
            
            return app.Use(next =>
            {
                return async context =>
                {
                    await pipeline.ExecuteAsync(context);
                    await next(context);
                };
            });
        }

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