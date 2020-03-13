using AutoFixture;
using Xunit;

namespace SCS.HomePhotos.Web.Test.Dtos
{
    public class PhotoDtoTests
    {
        private readonly Fixture _fixture = new Fixture();

        public PhotoDtoTests()
        {

        }

        [Fact]
        public void ToModel()
        {
            var dto = _fixture.Create<Dto.Photo>();
            var model = dto.ToModel();

            Assert.Equal(dto.PhotoId, model.PhotoId);
            Assert.Equal(dto.Name, model.Name);
            Assert.Equal(dto.FileName, model.FileName);
            Assert.Equal(dto.DateFileCreated, model.DateFileCreated);
            Assert.Equal(dto.DateTaken, model.DateTaken);
            Assert.Equal(dto.CacheFolder, model.CacheFolder);
        }

        [Fact]
        public void FromModel()
        {
            var model = _fixture.Create<Model.Photo>();
            var dto = new Dto.Photo(model);

            Assert.Equal(model.PhotoId, dto.PhotoId);
            Assert.Equal(model.Name, dto.Name);
            Assert.Equal(model.FileName, dto.FileName);
            Assert.Equal(model.DateFileCreated, dto.DateFileCreated);
            Assert.Equal(model.DateTaken, dto.DateTaken);
            Assert.Equal(model.CacheFolder, dto.CacheFolder);
        }
    }
}
