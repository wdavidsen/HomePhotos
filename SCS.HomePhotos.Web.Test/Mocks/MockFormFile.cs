using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Web.Test.Mocks
{
    public class MockFormFile : IFormFile
    {
        public string ContentType => "image/jpg";

        public string ContentDisposition => "form-data; name=\"files\"; filename=\"Whale Shark.jpg\"";

        public IHeaderDictionary Headers => new HeaderDictionary();

        public long Length => 2000;

        public string Name => "Whale Shark";

        public string FileName => $"{AppDomain.CurrentDomain.BaseDirectory}\\Images\\Whale Shark.jpg";

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
