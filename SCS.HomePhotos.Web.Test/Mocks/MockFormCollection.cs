using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SCS.HomePhotos.Web.Test.Mocks
{
    public class MockFormCollection : IFormCollection
    {
        private string[] _tags;
        private IFormFile _formFile;

        public MockFormCollection()
        {
            _tags = new string[] { "tag1", "tag2" };
        }
        public MockFormCollection(params string[] tags)
        {
            _tags = tags;
        }
        public MockFormCollection(IFormFile file, params string[] tags)
        {
            _tags = tags;
            _formFile = file;
        }

        public StringValues this[string key] => new StringValues(_tags);

        public int Count => 1;

        public IFormFileCollection Files => new MockFormFileCollection(_formFile);

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
}
