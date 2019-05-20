﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Miki.Framework.Commands.Permissions.Extensions;

namespace Miki.Framework.Commands.Permissions.Attributes
{
    public class RequiresPermissionAttribute : CommandRequirementAttribute
    {
        private PermissionLevel _level;

        public RequiresPermissionAttribute(PermissionLevel requiredPermission)
        {
            _level = requiredPermission;
        }

        public override async ValueTask<bool> CheckAsync(IContext e)
        {
            if (e.GetGuild() == null)
            {
                return _level <= 0;
            }

            var db = e.GetService<DbContext>();

            long authorId = (long)e.GetMessage().Author.Id;
            long guildId = (long)e.GetGuild().Id;
            
            var i = await db.GetUserPermissionLevelAsync(authorId, guildId);

            if (e.GetGuild().OwnerId == e.GetMessage().Author.Id
                && i < PermissionLevel.OWNER)
            {
                return _level <= PermissionLevel.OWNER;
            }

            return e.GetUserPermissions() >= _level; 
        }

        public override Task OnCheckFail(IContext e)
        {
            return Task.CompletedTask;
        }
    }
}
