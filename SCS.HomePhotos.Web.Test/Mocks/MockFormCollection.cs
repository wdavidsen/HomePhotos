using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SCS.HomePhotos.Web.Test.Mocks
{
    public class MockFormCollection : IFormCollection
    {
        public StringValues this[string key] => new StringValues(new string[] {"tag1", "tag2"});

        public int Count => 1;

        public IFormFileCollection Files => new MockFormFileCollection();

        public ICollection<string> Keys => new Collection<string> { "files", "tagList" };

        public bool ContainsKey(string key)
        {
            return Keys.Contains(key);
        }

        public IEnumerator<KeyValuePair<string, StringValues>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(string key, out StringValues value)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    public class MockFormFileCollection : IFormFileCollection
    {
        public IFormFile this[string name] => new MockFormFile();

        public IFormFile this[int index] => new MockFormFile();

        public int Count => 1;

        public IEnumerator<IFormFile> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public IFormFile GetFile(string name)
        {
            return new MockFormFile();
        }

        public IReadOnlyList<IFormFile> GetFiles(string name)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
