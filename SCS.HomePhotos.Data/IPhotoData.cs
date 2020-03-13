﻿using SCS.HomePhotos.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Data
{
    public interface IPhotoData : IDataBase
    {
        Task<IEnumerable<Photo>> GetPhotos(DateTime dateTakenStart, DateTime dateTakenEnd, bool descending = true, int pageNum = 0, int pageSize = 200);
        Task<IEnumerable<Photo>> GetPhotos(string[] tags, int pageNum = 0, int pageSize = 200);

        Task<Photo> SavePhoto(Photo photo);
        Task<PhotoTag> AssociatePhotoTag(int photoId, int tagId);
    }
}