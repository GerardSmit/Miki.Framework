using Miki.Framework.Models;

namespace Miki.Framework.Commands.Permissions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Miki.Framework.Commands.Permissions.Models;
    using Miki.Patterns.Repositories;

    /// <summary>
    /// Service that handles permissions, used as a basic ACL permission system.
    /// </summary>
    public class PermissionService : IPermissionService
    {
        private readonly IUnitOfWork unit;
        private readonly IAsyncRepository<Permission> repository;

        /// <summary>
        /// Initializes the permission service, use a scoped IUnitOfWork.
        /// </summary>
        public PermissionService(IUnitOfWork unit)
        {
            this.unit = unit ?? throw new ArgumentNullException(nameof(unit));
            this.repository = unit.GetRepository<Permission>();
        }

        /// <inheritdoc/>
        public async ValueTask DeleteAsync(Permission permission)
        {
            await repository.DeleteAsync(permission);
        }

        /// <inheritdoc/>
        public async ValueTask<bool> ExistsAsync(Permission permission)
        {
            var enumerable = await repository.ListAsync();
            if(enumerable is IQueryable<Permission> queryable)
            {
                return await queryable.AnyAsync(
                        x => x.EntityId == permission.EntityId
                             && x.CommandName == permission.CommandName
                             && x.GuildId == permission.GuildId
                             && x.PlatformId == permission.PlatformId)
                    .ConfigureAwait(false);
            }

            return enumerable.Any(
                x => x.EntityId == permission.EntityId
                     && x.CommandName == permission.CommandName
                     && x.GuildId == permission.GuildId
                     && x.PlatformId == permission.PlatformId);
        }

        /// <inheritdoc/>
        public ValueTask<Permission> GetPermissionAsync(
            string platformId,
            string entityId,
            string commandName,
            string guildId)
        {
            return repository.GetAsync(platformId, entityId, commandName, guildId);
        }

        /// <inheritdoc/>
        public async ValueTask<Permission> GetPriorityPermissionAsync(
            string platformId,
            string guildId,
            string commandName,
            string[] entityIds)
        {
            var enumerable = await repository.ListAsync();
            if (enumerable is IQueryable<Permission>)
            {
                return await enumerable.AsQueryable()
                    .Where(x => x.CommandName == commandName && x.GuildId == guildId && x.PlatformId == platformId)
                    .Where(x => entityIds.Contains(x.EntityId))
                    .OrderBy(x => x.Type)
                    .FirstOrDefaultAsync(x => x.Status != PermissionStatus.Default)
                    .ConfigureAwait(false);
            }
            return enumerable.Where(x => x.CommandName == commandName && x.GuildId == guildId && x.PlatformId == platformId)
                .Where(x => entityIds.Contains(x.EntityId))
                .OrderBy(x => x.Type)
                .FirstOrDefault(x => x.Status != PermissionStatus.Default);
        }

        /// <inheritdoc/>
        public async ValueTask<Permission> GetPriorityPermissionAsync(
            IContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var author = await context.Message.GetAuthorAsync().ConfigureAwait(false);

            if (author is IGuildUser guildUser && await author.Platform.IsAdministratorAsync(guildUser))
            {
                return new Permission
                {
                    EntityId = string.Empty,
                    CommandName = context.Executable.ToString(),
                    GuildId = guildUser.GuildId,
                    PlatformId = guildUser.Platform.Id,
                    Status = PermissionStatus.Allow,
                    Type = EntityType.User
                };
            }

            List<string> idList = new List<string>();
            
            if(context.Message != null)
            {
                idList.Add(author.Id);
                idList.Add(context.Message.ChannelId);
            }

            if(author is IGuildUser gu)
            {
                idList.Add(gu.GuildId);
            }

            // TODO (GerardSmit): Implement roles
            // if(context.GetMessage().Author is IDiscordGuildUser user
            //    && user.RoleIds != null
            //    && user.RoleIds.Any())
            // {
            //     idList.AddRange(user.RoleIds.Select(x => (long) x));
            // }

            return await GetPriorityPermissionAsync(context.Platform.Id, context.Message.GuildId, context.Executable.ToString(), idList.ToArray());
        }

        /// <inheritdoc/>
        public async ValueTask<IReadOnlyList<Permission>> ListPermissionsAsync(string platformId, string guildId)
        {
            var enumerable = await repository.ListAsync();
            if(enumerable is IQueryable<Permission>)
            {
                return await enumerable.AsQueryable()
                    .Where(x => x.PlatformId == platformId && x.GuildId == guildId).ToListAsync()
                    .ConfigureAwait(false);
            }
            return enumerable.Where(x => x.GuildId == guildId).ToList();
        }

        /// <inheritdoc/>
        public async ValueTask<IReadOnlyList<Permission>> ListPermissionsAsync(
            string platformId,
            string guildId,
            string commandName,
            params string[] entityFilter)
        {
            var enumerable = await repository.ListAsync();
            if(enumerable is IQueryable<Permission> queryable)
            {
                return await queryable.Where(x => x.GuildId == guildId)
                    .Where(x => entityFilter.Contains(x.EntityId))
                    .Where(x => x.CommandName == commandName)
                    .ToListAsync()
                    .ConfigureAwait(false);
            }

            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public async ValueTask<List<Permission>> ListPermissionsAsync(
            string platformId,
            string guildId,
            params string[] entityFilter)
        {
            var enumerable = await repository.ListAsync();
            if(enumerable is IQueryable<Permission> queryable)
            {
                return await queryable.Where(x => x.GuildId == guildId)
                    .Where(x => entityFilter.Contains(x.EntityId))
                    .ToListAsync()
                    .ConfigureAwait(false);
            }

            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public async ValueTask SetPermissionAsync(Permission permission)
        {
            if(await ExistsAsync(permission))
            {
                await repository.EditAsync(permission);
            }
            else
            {
                await repository.AddAsync(permission);
            }

            await unit.CommitAsync();
        }
    }
}
