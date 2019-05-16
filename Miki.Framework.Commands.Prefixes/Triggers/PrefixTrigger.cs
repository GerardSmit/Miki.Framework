﻿using Microsoft.EntityFrameworkCore;
using Miki.Cache;
using Miki.Discord.Common;
using Miki.Framework.Arguments;
using Miki.Framework.Commands;
using Miki.Framework.Commands.Pipelines;
using Miki.Framework.Commands.Prefixes.Models;
using Miki.Framework.Events.Triggers;
using System;
using System.Threading.Tasks;

namespace Miki.Framework.Events
{
	public class PrefixTrigger : ITrigger<IDiscordMessage>
	{
		public short Id { get; internal set; }
		public string Value { get; internal set; }
		public string DefaultValue { get; internal set; }
		public bool Changable { get; internal set; }
		public bool IsDefault { get; internal set; }

        public Func<IContext, Task> OnTriggerReceived { get; set; }

		public PrefixTrigger(string value, bool isDefault, bool changable = false)
		{
			Value = value;
			DefaultValue = value;
			IsDefault = isDefault;
			Changable = changable;
		}

        public async Task ChangeForGuildAsync(DbContext context, ICacheClient cache, ulong id, string prefix)
		{
			if (Changable)
			{
				long guildId = id.ToDbLong();

				Identifier i = await context.Set<Identifier>()
                    .FindAsync(guildId, DefaultValue);
				if (i == null)
				{
					await context.Set<Identifier>()
						.AddAsync(new Identifier() {
                            GuildId = guildId,
                            DefaultValue = DefaultValue,
                            Value = prefix
                        });
				}
				else
				{
					i.Value = prefix;
				}
				await context.SaveChangesAsync();
				await cache.UpsertAsync(GetCacheKey(id), prefix);
			}
		}

		private async Task<Identifier> CreateNewAsync(DbContext context, long id)
		{
			return (await context.Set<Identifier>()
				.AddAsync(new Identifier() { GuildId = id, DefaultValue = DefaultValue, Value = DefaultValue })).Entity;
		}

		private string GetCacheKey(ulong id)
			=> $"framework:prefix:{id}";

		public async Task<string> GetForGuildAsync(DbContext db, ICacheClient cache, ulong id)
		{
			if (Changable)
			{
				if (await cache.ExistsAsync(GetCacheKey(id)))
				{
					return await cache.GetAsync<string>(GetCacheKey(id));
				}

				string dbPrefix = await LoadFromDatabase(db, id);
				await cache.UpsertAsync(GetCacheKey(id), dbPrefix);
				return dbPrefix;
			}
			return DefaultValue;
		}

        public async Task<string> LoadFromDatabase(DbContext context, ulong id)
        {
            long guildId = id.ToDbLong();
            var identifier = await context.Set<Identifier>().FindAsync(guildId, DefaultValue);
            if (identifier == null)
            {
                identifier = await CreateNewAsync(context, guildId);
            }

            await context.SaveChangesAsync();
            return identifier.Value;
        }

        public async ValueTask<string> CheckTriggerAsync(IMutableContext e, IDiscordMessage packet)
        {
            var channel = await packet.GetChannelAsync();
            if(channel == null)
            {
                return null;
            }

            var prefix = DefaultValue;
            if (channel is IDiscordGuildChannel c)
            {
                prefix = await GetForGuildAsync(
                    e.GetService<DbContext>(), 
                    e.GetService<ICacheClient>(), 
                    c.GuildId);
            }

            var query = e.GetQuery();
            if (!query[0].StartsWith(prefix))
            {
                return null;
            }

            if (query[0].Length == prefix.Length)
            {
                query.RemoveAt(0);
            }
            else
            {
                query[0] = query[0].Substring(prefix.Length);
            }
            e.SetContext(CorePipelineStage.QueryArgumentKey, query);

            if (OnTriggerReceived != null)
            {
                await OnTriggerReceived(e);
            }
            return prefix;
        }
    }
}