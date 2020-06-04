using Miki.Framework.Hosting;
using Miki.Framework.Models;

namespace Miki.Framework.Commands.Filters
{
    using Miki.Framework.Commands.Pipelines;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class FilterPipelineStage : IPipelineStage
	{
		private readonly IEnumerable<IFilter> filters;

		public FilterPipelineStage(IEnumerable<IFilter> filters)
		{
			this.filters = filters;
		}

		public T GetFilterOfType<T>()
			where T : class, IFilter
		{
			if(filters == null || !filters.Any())
			{
				return default;
			}
			return filters.OfType<T>().FirstOrDefault();
		}

		public async ValueTask CheckAsync(IMessage data, IContext e, Func<ValueTask> next)
		{
			foreach(var f in filters)
			{
				if (!await f.CheckAsync(e))
				{
					return;
				}
			}
			await next();
		}
	}
}

namespace Miki.Framework.Commands
{
    using Miki.Framework.Commands.Filters;

    public static class Extensions
	{
		public static IBotApplicationBuilder UseFilter(this IBotApplicationBuilder b, IFilter f)
			=> b.UseFilters(f);

		public static IBotApplicationBuilder UseFilters(
            this IBotApplicationBuilder b, params IFilter[] filters)
			=> b.Use(new FilterPipelineStage(filters));
	}
}
