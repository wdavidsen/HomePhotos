using Xunit;

namespace SCS.HomePhotos.Web.Test.Services
{
    public class UploadTrackerTests
    {
        private readonly UploadTracker _uploadTracker;

        public UploadTrackerTests()
        {
            _uploadTracker = new UploadTracker();
        }

        [Fact]
        public void AddUpload()
        {
            var userName = "wdavidsen";
            var userName2 = "jdoe";
            var file1 = "temp\\test1.jpg";
            var file2 = "temp\\test2.jpg";
            var expectedCount = 0;

            _uploadTracker.AddUpload(userName, file1);
            _uploadTracker.AddUpload(userName, file2);
            _uploadTracker.AddUpload(userName2, file1);

            var actualCount = _uploadTracker.GetUploadCount(userName);

            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public void RemoveUpload()
        {
            var userName = "wdavidsen";
            var userName2 = "jdoe";
            var file1 = "temp\\test1.jpg";
            var file2 = "temp\\test2.jpg";
            var expectedCount = 1;

            _uploadTracker.AddUpload(userName, file1);
            _uploadTracker.AddUpload(userName, file2);
            _uploadTracker.AddUpload(userName2, file1);

            _uploadTracker.RemoveUpload(file2);

            var actualCount = _uploadTracker.GetUploadCount(userName);

            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public void IsProcessingDone()
        {
            var userName = "wdavidsen";
            var userName2 = "jdoe";
            var file1 = "temp\\test1.jpg";
            var file2 = "temp\\test2.jpg";

            _uploadTracker.AddUpload(userName, file1);
            _uploadTracker.AddUpload(userName, file2);
            _uploadTracker.AddUpload(userName2, file1);

            _uploadTracker.RemoveUpload(file1);
            _uploadTracker.RemoveUpload(file2);

            var isDone = _uploadTracker.IsProcessingDone(userName);

            Assert.True(isDone);
        }

        [Fact]
        public void Clear()
        {
            var userName = "wdavidsen";

            var file1 = "temp\\test1.jpg";
            var file2 = "temp\\test2.jpg";

            _uploadTracker.AddUpload(userName, file1);
            _uploadTracker.AddUpload(userName, file2);

            _uploadTracker.Clear();

            var count = _uploadTracker.GetUploadCount(userName);

            Assert.Equal(0, count);
        }
    }
}
