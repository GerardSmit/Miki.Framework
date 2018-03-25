﻿using Discord;
using System;
using System.Threading.Tasks;

namespace Miki.Framework.Events
{
    public delegate Task ProcessCommandDoneEvent(IMessage m, CommandEvent e, bool success, float time = 0.0f);

    public class CommandDoneEvent : CommandEvent
    {
        public ProcessCommandDoneEvent processEvent;

        public CommandDoneEvent() : base()
        {
        }
        public CommandDoneEvent(Action<CommandDoneEvent> e)
        {
            e.Invoke(this);
        }
    }
}