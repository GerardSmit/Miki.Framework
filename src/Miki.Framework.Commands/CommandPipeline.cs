using Miki.Framework.Models;

namespace Miki.Framework.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Miki.Framework.Commands.Pipelines;
    using Miki.Logging;

    public class CommandPipeline : IAsyncEventingExecutor<IMessage>
    {
        public IReadOnlyList<IPipelineStage> PipelineStages { get; }

        public Func<IExecutionResult<IMessage>, ValueTask> OnExecuted { get; set; }

        private readonly IServiceProvider services;

        internal CommandPipeline(
            IServiceProvider app,
            IReadOnlyList<IPipelineStage> stages)
        {
            PipelineStages = stages;
            services = app;
        }

        // TODO (velddev): Move IDiscordMessage to abstraction for a library-free solution.
        public async ValueTask ExecuteAsync(IMessage data)
        {
            using var contextObj = new ContextObject(services, data);

            await ExecuteAsync(contextObj);
        }

        public async Task ExecuteAsync(IContext contextObj)
        {
            var data = contextObj.Message;
            var sw = Stopwatch.StartNew();
            int index = 0;

            Func<ValueTask> nextFunc = null;

            ValueTask NextFunc()
            {
                if (contextObj == null)
                {
                    throw new InvalidOperationException("You're not allowed to nullify data");
                }

                if (index == PipelineStages.Count)
                {
                    return contextObj.Executable?.ExecuteAsync(contextObj) ?? default;
                }

                var stage = PipelineStages[index];
                index++;
                return stage?.CheckAsync(data, contextObj, nextFunc) ?? default;
            }

            nextFunc = NextFunc;

            Exception exception = null;
            try
            {
                await NextFunc();
            }
            catch (Exception e)
            {
                exception = e;
            }
            finally
            {
                if (this.OnExecuted != null)
                {
                    await OnExecuted(
                        new ExecutionResult<IMessage>(
                            contextObj, data, sw.ElapsedMilliseconds, exception));
                }
            }

            Log.Message($"request {contextObj.Executable} took {sw.ElapsedMilliseconds}ms.");
        }
    }
}
