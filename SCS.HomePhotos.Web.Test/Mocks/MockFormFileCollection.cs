using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SCS.HomePhotos.Web.Test.Mocks
{
    public sealed class MockFormFileCollection : IFormFileCollection
    {
        IFormFile _file;

        public MockFormFileCollection()
        {
            _file = new MockFormFile();
        }
        public MockFormFileCollection(IFormFile file)
        {
            if (file == null)
            {
                file = new MockFormFile();
            }
            _file = file;
        }

        public IFormFile this[string name] => _file;

        public IFormFile this[int index] => _file;

        public int Count => 1;

        public IEnumerator<IFormFile> GetEnumerator()
        {
            return new FileEnumerator(new IFormFile[] { _file });
        }

        public IFormFile GetFile(string name)
        {
            return _file;
        }

        public IReadOnlyList<IFormFile> GetFiles(string name)
        {
            return new IFormFile[] { _file };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    public sealed class FileEnumerator : IEnumerator<IFormFile>
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
