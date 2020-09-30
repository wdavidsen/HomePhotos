using System;
using System.Collections.Generic;
using System.Linq;

namespace SCS.HomePhotos.Web
{
    public class UploadTracker : IUploadTracker
    {
        private readonly Dictionary<string, int[]> _userCounts;
        private readonly List<UploadInfo> _items;

        public UploadTracker()
        {
            _items = new List<UploadInfo>();
            _userCounts = new Dictionary<string, int[]>();
        }

        public void AddUpload(string userName, string file)
        {
            var info = new UploadInfo
            {
                File = file,
                UserName = userName,
                Timestamp = DateTime.Now
            };

            if (_userCounts.ContainsKey(userName))
            {
                _userCounts[userName][0]++;
            }
            else
            {
                _userCounts.Add(userName, new int[] { 1, 0 });
            }

            _items.Add(info);
        }

        public void RemoveUpload(string file)
        {
            var item = _items.FirstOrDefault(i => i.File == file);

            if (item != null)
            {
                if (_userCounts.ContainsKey(item.UserName) && _userCounts[item.UserName][0] > 0)
                {
                    _userCounts[item.UserName][0]--;
                    _userCounts[item.UserName][1]++;
                }

                _items.Remove(item);
            }
        }

        public int GetUploadCount(string userName)
        {
            return _userCounts.ContainsKey(userName) ? _userCounts[userName][1] : 0;
        }

        public bool IsProcessingDone(string userName)
        {
            return _userCounts.ContainsKey(userName) && _userCounts[userName][0] == 0;
        }

        public void Clear()
        {
            _items.Clear();
        }
    }
}
