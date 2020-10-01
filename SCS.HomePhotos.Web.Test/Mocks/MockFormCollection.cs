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
        IFormFile file;

        public MockFormFileCollection()
        {
            file = new MockFormFile();
        }

        public IFormFile this[string name] => file;

        public IFormFile this[int index] => file;

        public int Count => 1;

        public IEnumerator<IFormFile> GetEnumerator()
        {
            return new FileEnumerator(new IFormFile[] { file });
        }

        public IFormFile GetFile(string name)
        {
            return file;
        }

        public IReadOnlyList<IFormFile> GetFiles(string name)
        {
            return new IFormFile[] { file };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    public class FileEnumerator : IEnumerator<IFormFile>
    {
        public IFormFile[] _files;
        private int _position = -1;

        public FileEnumerator(IFormFile[] files)
        {
            _files = files;
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public IFormFile Current
        {
            get
            {
                try
                {
                    return _files[_position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public bool MoveNext()
        {
            _position++;
            return (_position < _files.Length);
        }

        public void Reset()
        {
            _position = -1;
        }

        public void Dispose()
        {
        }
    }
}
