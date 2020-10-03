using Xunit;

namespace SCS.HomePhotos.Service.Test
{
    public class RegisterResultTests
    {
        private readonly RegisterResult _registerdResult;

        public RegisterResultTests()
        {
            _registerdResult = new RegisterResult();
        }

        [Fact]
        public void Success()
        {
            Assert.True(_registerdResult.Success);

            _registerdResult.PasswordNotStrong = true;
            Assert.False(_registerdResult.Success);
            _registerdResult.PasswordNotStrong = false;
            Assert.True(_registerdResult.Success);

            _registerdResult.UserNameTaken = true;
            Assert.False(_registerdResult.Success);
            _registerdResult.UserNameTaken = false;
            Assert.True(_registerdResult.Success);
        }
    }
}
