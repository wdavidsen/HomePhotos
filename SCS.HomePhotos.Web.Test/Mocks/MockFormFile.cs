using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Web.Test.Mocks
{
    public class MockFormFile : IFormFile
    {
        public string ContentType => "image/jpg";

        public string ContentDisposition => "content disposition";

        public IHeaderDictionary Headers => new HeaderDictionary();

        public long Length => 2000;

        public string Name => "MyFile";

        public string FileName => "c:\\temp\\MyFile.jpg";

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
            throw new NotImplementedException();
        }
    }
}
