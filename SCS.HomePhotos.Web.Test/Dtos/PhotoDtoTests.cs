using AutoFixture;
using System.Linq;
using Xunit;

namespace SCS.HomePhotos.Web.Test.Dtos
{
    public class PhotoDtoTests
    {
        private readonly Fixture _fixture;

        public PhotoDtoTests()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
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
            Assert.Equal(dto.ImageHeight, model.ImageHeight);
            Assert.Equal(dto.ImageWidth, model.ImageWidth);
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
            Assert.Equal(model.ImageHeight, dto.ImageHeight);
            Assert.Equal(model.ImageWidth, dto.ImageWidth);
        }
    }
}
