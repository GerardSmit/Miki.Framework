using Miki.Discord.Common;

namespace Miki.Framework
{
    using Logging;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;


    public class MessageBucketArgs
    {
        public Stream Attachment { get; set; }
        public MessageArgs Properties { get; set; }
        public IDiscordTextChannel Channel { get; set; }
    }
    
	public class MessageReference : IMessageReference<IDiscordMessage, MessageBucketArgs>
	{
		public List<Func<IDiscordMessage, Task>> Decorators { get; }
            = new List<Func<IDiscordMessage, Task>>();
        
        public MessageBucketArgs Arguments { get; }

        public MessageReference(MessageBucketArgs args)
        {
            Arguments = args;
        }

		public void PushDecorator(Func<IDiscordMessage, Task> fn)
		{
            Decorators.Add(fn);
		}
	}

    public class MessageWorker : IMessageWorker<IDiscordMessage, MessageBucketArgs>
	{
		private static readonly ConcurrentQueue<IMessageReference<IDiscordMessage, MessageBucketArgs>> QueuedMessages 
            = new ConcurrentQueue<IMessageReference<IDiscordMessage, MessageBucketArgs>>();

        private readonly ConfiguredTaskAwaitable workerTask;

        /// <summary>
        /// Amount of threads currently queueing messages.
        /// </summary>
        public int WorkerCount { get; }

        /// <summary>
        /// Initiates a message worker
        /// </summary>
        /// <param name="workerCount">amount of workers to be initialized, if none are given the default value is PROCESSOR_COUNT * 2</param>
        public MessageWorker(int? workerCount = null)
        {
            var count = workerCount.GetValueOrDefault(Environment.ProcessorCount * 2);
            for(int i = 0; i < count; i++)
            {
                WorkerCount++;
                workerTask = TickAsync().ConfigureAwait(false);
            }
            WorkerCount = count;
        }

		private static async Task TickAsync()
		{
			while(true)
			{
				if(QueuedMessages.IsEmpty)
				{
					await Task.Delay(50);
					continue;
				}

				if(QueuedMessages.TryDequeue(out IMessageReference<IDiscordMessage, MessageBucketArgs> msg))
				{
                    try
                    {
                        IDiscordMessage m;
                        if(msg.Arguments.Attachment == null)
                        {
                            m = await msg.Arguments.Channel.SendMessageAsync(
                                msg.Arguments.Properties.Content ?? "",
                                embed: msg.Arguments.Properties.Embed);
                        }
                        else
                        {
                            m = await msg.Arguments.Channel.SendFileAsync(
                                msg.Arguments.Attachment,
                                "file.png",
                                msg.Arguments.Properties.Content ?? "",
                                embed: msg.Arguments.Properties.Embed);
                            await msg.Arguments.Attachment.DisposeAsync();
                        }

                        if(msg.Decorators.Any())
                        {
                            await ProcessDecoratorsAsync(msg, m);
                        }
                    }
                    catch(Exception e)
					{
						Log.Error(e);
					}
				}
			}
		}

        private static async Task ProcessDecoratorsAsync(
            IMessageReference<IDiscordMessage, MessageBucketArgs> msgRef,
            IDiscordMessage msg)
        {
            foreach(var x in msgRef.Decorators)
            {
                await x(msg);
            }
        }

        /// <inheritdoc />
        public IMessageReference<IDiscordMessage, MessageBucketArgs> CreateRef(MessageBucketArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }
            return new MessageReference(args);
        }

        /// <inheritdoc />
        public void Execute(IMessageReference<IDiscordMessage, MessageBucketArgs> args)
        {
            QueuedMessages.Enqueue(args);
        }
    }
}