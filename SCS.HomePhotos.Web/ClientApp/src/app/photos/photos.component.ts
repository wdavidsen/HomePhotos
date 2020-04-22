import { Component, OnInit, OnDestroy } from '@angular/core';
import { PhotosService } from '../services/photos.service';
import { Thumbnail, Photo } from '../models';
import { map } from 'rxjs/operators';
import { ActivatedRoute } from '@angular/router';
import { SearchService, OrganizeService } from '../services';
import { PageInfoService } from '../services/page-info.service';
import { Subscription } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { PhotoTaggerComponent } from './photo-tagger.component';

declare const blueimp: any;

@Component({
  selector: 'app-photos',
  templateUrl: './photos.component.html',
  styleUrls: ['./photos.component.css']
})
export class PhotosComponent implements OnInit, OnDestroy {
  thumbHeight = 100;
  thumbnails: Thumbnail[] = [];
  tagName: string;
  organizeMode = false;
  taggerModalRef: BsModalRef;

  private organizeSubscription: Subscription;
  private searchSubscription: Subscription;

  constructor(private photosService: PhotosService,
    private route: ActivatedRoute,
    private pageInfoService: PageInfoService,
    private searchService: SearchService,
    private organizeService: OrganizeService,
    private toastr: ToastrService,
    private modalService: BsModalService) {

    }

  ngOnInit() {
    this.searchService.setHidden(false);

    this.route.paramMap.subscribe(params => {
      this.tagName = params.get('tagName');
      console.log(`Received tag: ${this.tagName}`);

      if (this.tagName) {
        this.pageInfoService.setTitle(this.tagName);

        this.photosService.getPhotosByTag(this.tagName)
          .pipe(map(photos => this.photosToThumbnails(photos)))
          .subscribe((thumbs => this.thumbnails =  thumbs));
      }
      else {
        this.pageInfoService.setTitle('Latest Photos');

        this.photosService.getLatest()
          .pipe(map(photos => this.photosToThumbnails(photos)))
          .subscribe((thumbs => this.thumbnails =  thumbs));
      }
    });

    this.searchSubscription = this.searchService.getKeywords()
      .subscribe(keywords => {
        if (keywords) {
          console.log(`Received search keywords: ${keywords}`);
          this.photosService.searchPhotos(keywords)
            .pipe(map(photos => this.photosToThumbnails(photos)))
            .subscribe((thumbs => this.thumbnails =  thumbs));
        }
        else {
          this.photosService.getLatest()
            .pipe(map(photos => this.photosToThumbnails(photos)))
            .subscribe((thumbs => this.thumbnails =  thumbs));
        }
      });

    this.organizeSubscription = this.organizeService.getEnabled()
      .subscribe(enabled => {
        this.organizeMode = enabled;
        this.clearSelections();
      });
  }

  ngOnDestroy() {
    this.searchSubscription.unsubscribe();
    this.organizeSubscription.unsubscribe();
  }

  select(thumbnail: Thumbnail) {
    if (this.organizeMode) {
      thumbnail.selected = !thumbnail.selected;
    }
    else {
      this.showLightbox();
    }
  }

  showLightbox() {
    const options = {
        event: window.event,
    };

    const images: any[] = [];
    this.thumbnails.forEach(thumb => {
      images.push({
        href: `${thumb.thumbUrl}?type=full`,
        type: 'image/jpeg',
        thumbnail: thumb.thumbUrl
      });
    });

    blueimp.Gallery(images, options);
  }

  getSelectedThumbnails(): Thumbnail[] {
    return this.thumbnails.filter(thumb => thumb.selected);
  }

  clearSelections(): void {
    this.thumbnails.forEach(thumb => thumb.selected = false);
  }

  showTagTool() {
    const selections = this.getSelectedThumbnails();

    if (selections.length) {
      const initialState = {
        title: 'Photo Tagger',
        photoIds: selections.map(s => s.photoId)
      };

      this.taggerModalRef = this.modalService.show(PhotoTaggerComponent, {initialState});
    }
  }

  selectAll() {
    this.thumbnails.forEach(thumb => thumb.selected = true);
  }

  private photosToThumbnails(photos: Photo[]): Thumbnail[] {
    const thumbs = new Array<Thumbnail>();
    photos.forEach(photo => thumbs.push(this.photoToThumbnail(photo)));

    return thumbs;
  }

  private photoToThumbnail(photo: Photo): Thumbnail {
    const thumb = new Thumbnail();
    const heightWidth = this.calculateThumbSize(photo, this.thumbHeight);

    thumb.photoId = photo.photoId;
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
