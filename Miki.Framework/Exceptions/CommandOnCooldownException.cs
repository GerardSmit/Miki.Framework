﻿using Miki.Framework.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Framework.Exceptions
{
	public class CommandOnCooldownException : CommandException
	{
		public override string Resource => "error_default_command";

		public CommandOnCooldownException(CommandEvent e) : base(e)
		{ }
	}
}
