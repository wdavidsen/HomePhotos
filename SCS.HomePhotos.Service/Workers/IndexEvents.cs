using System;

namespace SCS.HomePhotos.Service.Workers
{
    public class IndexEvents : IIndexEvents
    {
        public Action IndexStarted { get; set; }
        public Action IndexCompleted { get; set; }
    }
}
