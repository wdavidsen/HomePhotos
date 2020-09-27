using System;

namespace SCS.HomePhotos.Service.Workers
{
    public interface IIndexEvents
    {
        Action IndexCompleted { get; set; }
        Action IndexStarted { get; set; }
    }
}