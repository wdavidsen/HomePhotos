using System;

namespace SCS.HomePhotos.Service.Workers
{
    public interface IQueueEvents
    {
        Action<TaskCompleteInfo> ItemProcessed { get; set; }
    }
}