﻿using Miki.Common;
using Miki.Discord.Common;
using Miki.Framework.Events.Commands;
using Miki.Framework.Events.Filters;
using Miki.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Miki.Framework.Events
{
	public class SimpleCommandHandler : CommandHandler
	{
		public IReadOnlyList<CommandEvent> Commands => map.Commands;
		public IReadOnlyList<Module> Modules => map.Modules;

		public SimpleCommandHandler(CommandMap map)
		{
			this.map = map;
		}

		public override async Task CheckAsync(MessageContext context)
		{
			try
			{
				Stopwatch sw = Stopwatch.StartNew();

				await base.CheckAsync(context);

				foreach (PrefixInstance prefix in Prefixes.Values)
				{
					string identifier = prefix.DefaultValue;

					if ((await context.message.GetChannelAsync()) is IDiscordGuildChannel channel)
					{
						identifier = await prefix.GetForGuildAsync(channel.GuildId);
					}

					if (!context.message.Content.StartsWith(identifier))
					{
						continue;
					}

					string command = Regex.Replace(context.message.Content, @"\r\n?|\n", "")
						.Substring(identifier.Length)
						.Split(' ')
						.First();

					CommandEvent eventInstance = map.GetCommandEvent(command);

					if (eventInstance == null)
					{
						return;
					}

					if ((await GetUserAccessibility(context.message)) >= eventInstance.Accessibility)
					{
						if (await eventInstance.IsEnabled((await context.message.GetChannelAsync()).Id))
						{
							await eventInstance.Check(context, identifier);
							await OnMessageProcessed(eventInstance, context.message, sw.ElapsedMilliseconds);
						}
					}
				}
			}
			catch(Exception e)
			{
				Log.Error(e);
			}
		}

		// TODO: rework this
        public async Task<EventAccessibility> GetUserAccessibility(IDiscordMessage e)
        {
			if (e.Author.Id == 121919449996460033)
			{
				return EventAccessibility.DEVELOPERONLY;
			}

			IDiscordChannel channel = await e.GetChannelAsync();

			if (channel is IDiscordGuildChannel guildChannel)
			{
				IDiscordGuildUser u = await (await guildChannel.GetGuildAsync()).GetUserAsync(e.Author.Id);
				if ((await guildChannel.GetPermissionsAsync(u)).HasFlag(GuildPermission.ManageRoles))
				{
					return EventAccessibility.ADMINONLY;
				}
			}

			return EventAccessibility.PUBLIC;
        }
    }
}