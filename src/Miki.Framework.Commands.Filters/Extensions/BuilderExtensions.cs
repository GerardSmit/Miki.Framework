using System;
using Microsoft.Extensions.DependencyInjection;
using Miki.Framework.Hosting;

namespace Miki.Framework.Commands.Filters
{
    public static class BuilderExtensions
    {
        public static IBotApplicationBuilder UseFilters(this IBotApplicationBuilder app, params IFilter[] filters)
        {
            return app.Use(new FilterPipelineStage(filters));
        }
        
        public static IBotApplicationBuilder UseFilters<T>(this IBotApplicationBuilder app)
            where T : IFilter
        {
            return app.Use(new FilterPipelineStage(new IFilter[]
            {
                app.ApplicationServices.GetOrCreateService<T>()
            }));
        }
        
        public static IBotApplicationBuilder UseFilters<T1, T2>(this IBotApplicationBuilder app)
            where T1 : IFilter
            where T2 : IFilter
        {
            return app.Use(new FilterPipelineStage(new IFilter[]
            {
                app.ApplicationServices.GetOrCreateService<T1>(),
                app.ApplicationServices.GetOrCreateService<T2>()
            }));
        }
        
        public static IBotApplicationBuilder UseFilters<T1, T2, T3>(this IBotApplicationBuilder app)
            where T1 : IFilter
            where T2 : IFilter
            where T3 : IFilter
        {
            return app.Use(new FilterPipelineStage(new IFilter[]
            {
                app.ApplicationServices.GetOrCreateService<T1>(),
                app.ApplicationServices.GetOrCreateService<T2>(),
                app.ApplicationServices.GetOrCreateService<T3>()
            }));
        }
    }
}