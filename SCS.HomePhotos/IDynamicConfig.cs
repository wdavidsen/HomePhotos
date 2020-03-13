using System;
using System.ComponentModel;

namespace SCS.HomePhotos
{
    public interface IDynamicConfig : IConfig
    {
        event PropertyChangedEventHandler PropertyChanged;

        bool TrackChanges { get; set; }

        IDynamicConfig GetDefault();
    }
}