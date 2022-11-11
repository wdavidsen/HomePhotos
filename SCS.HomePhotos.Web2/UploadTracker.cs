namespace SCS.HomePhotos.Web
{
    /// <summary>
    /// Image file upload tracker.
    /// </summary>
    public class UploadTracker : IUploadTracker
    {
        private readonly Dictionary<string, int[]> _userCounts;
        private readonly List<UploadInfo> _items;
        private readonly Timer _timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadTracker"/> class.
        /// </summary>
        public UploadTracker()
        {
            _items = new List<UploadInfo>();
            _userCounts = new Dictionary<string, int[]>();

            var periodMs = 1000 * 60 * 5; // 5 mins
            _timer = new Timer(ClearOldItems, null, periodMs, periodMs);
        }

        /// <summary>
        /// Increments the add upload counter.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="file">The file.</param>
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

        /// <summary>
        /// Removes an upload from the counter.
        /// </summary>
        /// <param name="file">The uploaded file.</param>
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

        /// <summary>
        /// Gets the upload count.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        public int GetUploadCount(string userName)
        {
            return _userCounts.ContainsKey(userName) ? _userCounts[userName][1] : 0;
        }

        /// <summary>
        /// Determines whether upload processing done for user.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns>
        ///   <c>true</c> if upload processing done for user; otherwise, <c>false</c>.
        /// </returns>
        public bool IsProcessingDone(string userName)
        {
            return _userCounts.ContainsKey(userName) && _userCounts[userName][0] == 0;
        }

        /// <summary>
        /// Clears the upload count.
        /// </summary>
        public void Clear()
        {
            _items.Clear();
        }

        /// <summary>
        /// Clears the old items.
        /// </summary>
        /// <param name="state">The state.</param>
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
