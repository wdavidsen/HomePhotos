using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Web.Test.Mocks
{
    public class MockFormFile : IFormFile
    {
        private string _fileName;
        private string _contentType;

        public MockFormFile()
        {
            _fileName = "Whale Shark.jpg";
            _contentType = "image/jpg";
        }
        public MockFormFile(string fileName)
        {
            _fileName = fileName;
            _contentType = $"image/{Path.GetFileNameWithoutExtension(fileName).TrimStart('.')}";
        }

        public string ContentType
        {
            get { return _contentType; }            
        }

        public string ContentDisposition => $"form-data; name=\"files\"; filename=\"{_fileName}\"";

        public IHeaderDictionary Headers => new HeaderDictionary();

        public long Length => 2000;

        public string Name => Path.GetFileNameWithoutExtension(_fileName);

        public string FileName => $"{AppDomain.CurrentDomain.BaseDirectory}\\Images\\{_fileName}";

        public void CopyTo(Stream target)
        {
            throw new NotImplementedException();
        }

        public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Stream OpenReadStream()
        {
            return new FileStream(FileName, FileMode.Open, FileAccess.Read);
        }
    }
}
