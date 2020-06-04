using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Miki.Framework.Models;

namespace Miki.Framework
{
    public interface IMessageReference<TMessage>
        where TMessage : class
    {
        List<Func<TMessage, Task>> Decorators { get; }

        void PushDecorator(Func<TMessage, Task> fn);
    }
    
    /// <summary>
    /// Message reference to use while a message is being queued to 
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <typeparam name="TArgs"></typeparam>
    public interface IMessageReference<TMessage, out TArgs> : IMessageReference<TMessage>
        where TMessage : class
    {
        TArgs Arguments { get; }
    }
}