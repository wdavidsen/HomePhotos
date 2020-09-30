using System;

namespace SCS.HomePhotos.Service.Workers
{
    public class QueueEvents : IQueueEvents
    {
        public Action<TaskCompleteInfo> ItemProcessed { get; set; }
    }
}
