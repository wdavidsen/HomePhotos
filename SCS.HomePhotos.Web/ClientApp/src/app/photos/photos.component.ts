import { Component, OnInit } from '@angular/core';
import { PhotosService } from '../services/photos.service';
import { Thumbnail, Photo } from '../models';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-photos',
  templateUrl: './photos.component.html',
  styleUrls: ['./photos.component.css']
})
export class PhotosComponent implements OnInit {
  thumbHeight = 100;
  thumbnails: Thumbnail[];

  constructor(private photosService: PhotosService) { }

  ngOnInit() {

    this.photosService.getLatest()
      .pipe(map(photos => this.photosToThumbnails(photos)))
      .subscribe((thumbs => this.thumbnails =  thumbs));
  }

  private photosToThumbnails(photos: Photo[]): Thumbnail[] {
    const thumbs = new Array<Thumbnail>();
    photos.forEach(photo => thumbs.push(this.photoToThumbnail(photo)));

    return thumbs;
  }

  private photoToThumbnail(photo: Photo): Thumbnail {
    const thumb = new Thumbnail();
    const heightWidth = this.calculateThumbSize(photo, this.thumbHeight);

    thumb.selected = false;
    thumb.thumbUrl = `/photo-image/${photo.cacheFolder}/${photo.fileName}`;
    thumb.thumbHeight = heightWidth.height;
    thumb.thumbWidth = heightWidth.width;

    return thumb;
  }

  private calculateThumbSize(photo: Photo, height: number) {
    let width = height;
    let extraPerThumb = 0;
    const windowWidth = window.innerWidth;

    if (windowWidth < 768) {
        // display as center cropped square maximizing all space (for small screens)
        const thumbsPerLine = Math.floor(windowWidth / (width + 4));
        const extra = windowWidth - (thumbsPerLine * (width + 4));
        extraPerThumb = Math.floor(extra / thumbsPerLine);
    }
    else {
        // display with normal image aspect ratio (no cropping)
        const ratio = photo.imageWidth / photo.imageHeight;
        width = Math.floor(height * ratio);
    }

    return { height: height + extraPerThumb, width: width + extraPerThumb };
  }
}
