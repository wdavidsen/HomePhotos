using Xunit;

namespace SCS.HomePhotos.Service.Test
{
    public class ChangePasswordResultTests
    {
        private readonly ChangePasswordResult _changePasswordResult;

        public ChangePasswordResultTests()
        {
            _changePasswordResult = new ChangePasswordResult();
        }

        [Fact]
        public void Success()
        {
            Assert.True(_changePasswordResult.Success);

            _changePasswordResult.PasswordUsedPreviously = true;
            Assert.False(_changePasswordResult.Success);
            _changePasswordResult.PasswordUsedPreviously = false;
            Assert.True(_changePasswordResult.Success);
        }
    }
}
