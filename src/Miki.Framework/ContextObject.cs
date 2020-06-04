using Miki.Framework.Models;

namespace Miki.Framework
{
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Collections.Generic;

	/// <summary>
	/// Session context for a single command. Keeps data and services for this specific session.
	/// </summary>
    public interface IContext
	{
		/// <summary>
		/// The command executed in this current session.
		/// </summary>
		IExecutable Executable { get; set; }
		
		/// <summary>
		/// The platform that created this context.
		/// </summary>
		IPlatform Platform { get; }
		
		/// <summary>
		/// Incoming message.
		/// </summary>
		IMessage Message { get; }

		/// <summary>
		/// Current request services.
		/// </summary>
		IServiceProvider RequestServices { get; }

		/// <summary>
		/// Context objects are used for specific session-only objects that are added through pipeline
		/// objects.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		object GetContext(string id);
		
		void SetContext(string id, object value);

        /// <summary>
        /// Used to retrieve services built in <see cref="MikiApp"/>
        /// </summary>
		object GetService(Type t);
	}

    /// <inheritdoc cref="IContext" />
    public class ContextObject : IContext, IDisposable
	{
		private readonly Dictionary<string, object> contextObjects;
		private readonly IServiceScope scope;

		public IPlatform Platform => Message.Platform;
		
		public IMessage Message { get; }

		/// <summary>
		/// Service collection of the current context.
		/// </summary>
        public IServiceProvider RequestServices
			=> scope.ServiceProvider;

		/// <summary>
		/// Current set Executable.
		/// </summary>
		public IExecutable Executable { get; set; }

		/// <summary>
		/// Creates a scoped context object
		/// </summary>
        public ContextObject(IServiceProvider p, IMessage message)
		{
			Message = message;
			contextObjects = new Dictionary<string, object>();
			scope = p.CreateScope();
		}

        /// <inheritdoc/>
        public void Dispose()
		{
			scope.Dispose();
		}

        /// <inheritdoc/>
        public object GetContext(string id)
		{
			if(contextObjects.TryGetValue(id, out var value))
			{
				return value;
			}
			return default;
		}

        /// <inheritdoc/>
		public object GetService(Type t)
			=> scope.ServiceProvider.GetService(t);

        /// <inheritdoc/>
        public void SetContext(string id, object value)
		{
			if(contextObjects.ContainsKey(id))
			{
				contextObjects[id] = value;
			}
			else
			{
				contextObjects.Add(id, value);
			}
		}

        /// <inheritdoc/>
        public void SetExecutable(IExecutable exec)
		{
			Executable = exec;
		}
	}
}
