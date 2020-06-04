namespace Miki.Framework
{
    public interface IMessageWorker<TMessage, TArgs>
        where TMessage : class
    {
        /// <summary>
        /// Creates a reference to queue in the worker in the future.
        /// </summary>
        IMessageReference<TMessage, TArgs> CreateRef(TArgs args);
        
        /// <summary>
        /// Queues a reference in the worker.
        /// </summary>
        void Execute(IMessageReference<TMessage, TArgs> args);
    }
}