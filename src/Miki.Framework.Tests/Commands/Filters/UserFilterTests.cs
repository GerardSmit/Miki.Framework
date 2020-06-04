using Miki.Framework.Models;

namespace Miki.Framework.Tests.Commands.Filters
{
    using Miki.Framework.Commands.Filters;
    using System.Threading.Tasks;
    using Moq;
    using Xunit;

    public class UserFilterTests
    {
        private readonly UserFilter userFilter;

        private const string Platform = "discord";
        private const string OtherPlatform = "twitch";
        private const string BannedUser = "0";
        private const string ValidUser = "1";

        public UserFilterTests()
        {
            userFilter = new UserFilter();
            userFilter.Rules.Add(new UserRule(Platform, BannedUser));
        }

        [Theory]
        [InlineData(Platform, BannedUser, false)]
        [InlineData(Platform, ValidUser, true)]
        [InlineData(OtherPlatform, BannedUser, true)]
        [InlineData(OtherPlatform, ValidUser, true)]
        public async Task UserIsAllowedTestAsync(string platform, string userId, bool allowed)
        {
            var isAllowed = await userFilter.CheckAsync(NewContext(userId, platform));
            Assert.Equal(allowed, isAllowed);
        }

        private static IContext NewContext(string userId, string platformId)
        {
            var platform = new Mock<IPlatform>();
            platform.Setup(x => x.Type)
                .Returns(platformId);
            
            var author = new Mock<IUser>();
            author.Setup(x => x.Id)
                .Returns(userId);
            
            author.Setup(x => x.Platform)
                .Returns(platform.Object);

            var message = new Mock<IMessage>();
            message.Setup(x => x.GetAuthorAsync())
                .Returns(Task.FromResult(author.Object));

            var context = new Mock<IContext>();
            context.Setup(x => x.Message)
                .Returns(message.Object);

            return context.Object;
        }
    }
}
