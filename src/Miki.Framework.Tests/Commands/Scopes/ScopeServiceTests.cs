namespace Miki.Framework.Tests.Commands.Scopes
{
    using System;
    using System.Threading.Tasks;
    using Framework.Commands.Scopes;
    using Framework.Commands.Scopes.Models;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    public class ScopeServiceTests : BaseEntityTest<Scope>
    {
        private const string PlatformId = "discord:0";
        private const string ValidScope = "scope.testing";
        private const string ValidId = "0";

        public ScopeServiceTests()
        {
            using var context = NewDbContext();
            context.Set<Scope>()
                .Add(new Scope
                {
                    PlatformId = PlatformId,
                    ScopeId = ValidScope,
                    UserId = ValidId
                });
            context.SaveChanges();
        }

        [Fact]
        public async Task AddScopeTestAsync()
        {
            const string newId = "1";
            const string newScope = "scope.new";

            await using (var context = NewContext())
            {
                var service = new ScopeService(context);
                await service.AddScopeAsync(new Scope
                {
                    UserId = newId,
                    ScopeId = newScope,
                    PlatformId = PlatformId
                });
                await context.CommitAsync();
            }

            await using (var context = NewContext())
            {
                var service = new ScopeService(context);
                var result = await service.HasScopeAsync(PlatformId, newId, new [] {newScope});
                Assert.True(result);
            }
        }

        [Theory]
        [InlineData(ValidId, ValidScope, true)]
        [InlineData("0", "invalid.scope", false)]
        public async Task HasScopeTestAsync(string id, string scope, bool expected)
        {
            await using var context = NewContext();
            var service = new ScopeService(context);

            bool hasScope = await service.HasScopeAsync(PlatformId, id, new [] {scope});

            Assert.Equal(expected, hasScope);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Entity<Scope>()
                .HasKey(x => new {x.UserId, x.ScopeId});
        }
    }
}
