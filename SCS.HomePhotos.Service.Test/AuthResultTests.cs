using Xunit;

namespace SCS.HomePhotos.Service.Test
{
    public class AuthResultTests
    {
        private readonly AuthResult _authResult;

        public AuthResultTests()
        {
            _authResult = new AuthResult();
        }

        [Fact]
        public void Success()
        {
            Assert.True(_authResult.Success);

            _authResult.MustChangePassword = true;
            Assert.False(_authResult.Success);
            _authResult.MustChangePassword = false;
            Assert.True(_authResult.Success);

            _authResult.MustChangePassword = true;
            Assert.False(_authResult.Success);
            _authResult.MustChangePassword = false;
            Assert.True(_authResult.Success);

            _authResult.UserDisabled = true;
            Assert.False(_authResult.Success);
            _authResult.UserDisabled = false;
            Assert.True(_authResult.Success);

            _authResult.UserNotExists = true;
            Assert.False(_authResult.Success);
            _authResult.UserNotExists = false;
            Assert.True(_authResult.Success);

            _authResult.AttemptsExceeded = true;
            Assert.False(_authResult.Success);
            _authResult.AttemptsExceeded = false;
            Assert.True(_authResult.Success);
        }
    }
}
