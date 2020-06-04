﻿using Miki.Framework.Models;

 namespace Miki.Framework.Commands.Permissions
 {
     using Miki.Framework.Commands.Permissions.Attributes;
     using Miki.Framework.Commands.Permissions.Models;
     using Miki.Framework.Commands.Pipelines;
     using Miki.Logging;
     using System;
     using System.Diagnostics.CodeAnalysis;
     using System.Linq;
     using System.Threading.Tasks;

     /// <summary>
     /// Pipeline stage for Miki's Command Pipeline system, checks if the permissions are valid for this
     /// executable.
     /// </summary>
     public class PermissionPipelineStage : IPipelineStage
     {
         private readonly PermissionService service;

         internal PermissionPipelineStage(PermissionService service)
         {
             this.service = service;
         }

         /// <inheritdoc/>
         public async ValueTask CheckAsync(IMessage data, [NotNull] IContext e, [NotNull] Func<ValueTask> next)
         {
             if(e?.Executable == null)
             {
                 Log.Debug("No executable found to perform permission check on.");
                 return;
             }

             var user = await e.Message.GetAuthorAsync().ConfigureAwait(false);
             if (user is IGuildUser guildUser)
             {
                 var permission = await service.GetPriorityPermissionAsync(e);
                 if(permission == null)
                 {
                     var defaultStatus = FetchPermissionStatusFrom(e.Executable);
                     permission = new Permission
                     {
                         GuildId = guildUser.GuildId,
                         PlatformId = guildUser.Platform.Id,
                         CommandName = e.Executable.ToString(),
                         EntityId = string.Empty,
                         Status = defaultStatus,
                         Type = 0
                     };
                 }

                 Log.Debug(permission.ToString());

                 if(permission.Status == PermissionStatus.Allow)
                 {
                     await next();
                 }
             }
             else
             {
                 if(FetchPermissionStatusFrom(e.Executable) == PermissionStatus.Deny)
                 {
                     throw new InvalidOperationException(
                         "Denied request due to default setting set to Deny");
                 }

                 await next();
             }
         }

         private PermissionStatus FetchPermissionStatusFrom(IExecutable executable)
         {
             if(!(executable is Node node))
             {
                 return PermissionStatus.Allow;
             }

             var defaultPermissionAtrrib = (DefaultPermissionAttribute) node.Attributes
                 .FirstOrDefault(x => x is DefaultPermissionAttribute);

             if(defaultPermissionAtrrib != null)
             {
                 return defaultPermissionAtrrib.Status;
             }

             return PermissionStatus.Allow;
         }
     }
 }

 namespace Miki.Framework.Commands
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Miki.Framework.Commands.Permissions;

    /// <summary>
    /// Helper functions for the PermissionPipelineStage.
    /// </summary>
    public static class PermissionExtensions
	{
        /// <summary>
        /// Initializes the permissions system at this index on your CommandPipeline. Permissions will
        /// give users a way to manage their entire command infrastructure in a ACL kind of manner.
        ///
        /// This stage requires you to already have set an Executable to work properly.
        /// </summary>
		public static CommandPipelineBuilder UsePermissions(this CommandPipelineBuilder builder)
		{
            if(builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.UseStage(
                new PermissionPipelineStage(
                    builder.Services.GetRequiredService<PermissionService>()));
			return builder;
		}
	}
}
