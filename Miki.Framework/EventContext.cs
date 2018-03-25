﻿using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Framework.Events
{
	public class EventContext
	{
		public Args Arguments;

		public CommandHandler commandHandler;

		public IMessage message;

		public IUser Author => message.Author;

		public IMessageChannel Channel => message.Channel;

		public IGuild Guild => ((message as IUserMessage).Channel as IGuildChannel).Guild;

		public EventSystem EventSystem;
	}
}
