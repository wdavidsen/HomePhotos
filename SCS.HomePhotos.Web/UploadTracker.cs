using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SCS.HomePhotos.Web
{
    public class UploadTracker : IUploadTracker
    {
        private readonly Dictionary<string, int[]> _userCounts;
        private readonly List<UploadInfo> _items;
        private readonly Timer _timer;

        public UploadTracker()
        {
            _items = new List<UploadInfo>();
            _userCounts = new Dictionary<string, int[]>();

            var periodMs = 1000 * 60 * 5; // 5 mins
            _timer = new Timer(ClearOldItems, null, periodMs, periodMs);
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

        private void ClearOldItems(object state)
        {
            var oldItems = _items.Where(i => i.Timestamp.AddMinutes(15) < DateTime.Now);
            var groupedItems = oldItems.GroupBy(i => i.UserName);

            foreach (var group in groupedItems)
            {
                var userName = group.Key;
                _userCounts[userName] = new int[] { 0, 0 };

                foreach (var item in group)
                {
                    _items.Remove(item);
                }
            }
        }
    }
}
