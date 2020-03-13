using AutoFixture;
using Xunit;

namespace SCS.HomePhotos.Web.Test.Dtos
{
    public class TagDtoTests
    {
        private readonly Fixture _fixture = new Fixture();

        public TagDtoTests()
        {

        }

        [Fact]
        public void ToModel()
        {
            var dto = _fixture.Create<Dto.Tag>();
            var model = dto.ToModel() as Model.TagStat;

            Assert.Equal(dto.TagId, model.TagId);
            Assert.Equal(dto.TagName, model.TagName);
            Assert.Equal(dto.PhotoCount, model.PhotoCount);
        }

        [Fact]
        public void FromModel()
        {
            var model = _fixture.Create<Model.TagStat>();
            var dto = new Dto.Tag(model);

            Assert.Equal(model.TagId, dto.TagId);
            Assert.Equal(model.TagName, dto.TagName);
            Assert.Equal(model.PhotoCount, dto.PhotoCount);
        }
    }
}
