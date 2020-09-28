import { Component, OnInit, OnDestroy, HostListener } from '@angular/core';
import { PhotosService } from '../services/photos.service';
import { Thumbnail, Photo, User } from '../models';
import { map, debounce } from 'rxjs/operators';
import { ActivatedRoute } from '@angular/router';
import { SearchService, OrganizeService, AuthenticationService, UserSettingsService } from '../services';
import { PageInfoService } from '../services/page-info.service';
import { Subscription, Subject, timer } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { PhotoTaggerComponent } from './photo-tagger.component';
import { ScrollService } from '../services/scroll.service';
import { UserSettings } from '../models/user-settings';

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

  private keywords: string;
  private pageNum = 1;
  private mode = 1;
  private previousScroll = 0;
  private organizeSubscription: Subscription;
  private searchSubscription: Subscription;
  private bottomScrollSubscription: Subscription;
  private userSettingsSubscription: Subscription;
  private scrollSubject = new Subject<number>();
  private currentUser: User;
  private userSettings: UserSettings;

  constructor(private photosService: PhotosService,
    private route: ActivatedRoute,
    private pageInfoService: PageInfoService,
    private searchService: SearchService,
    private organizeService: OrganizeService,
    private userSettingsService: UserSettingsService,
    private toastr: ToastrService,
    private modalService: BsModalService,
    private scrollService: ScrollService,
    private authenticationService: AuthenticationService) {
      this.authenticationService.currentUser.subscribe(user => {
        this.currentUser = user;
        this.userSettings = this.userSettingsService.userSettings;
        this.thumbHeight = this.getThumbHeight(this.userSettings.thumbnailSize);
      });
    }

  @HostListener('document:scroll', ['$event'])
  onScroll(event: KeyboardEvent) {
    const docElem = document.documentElement;
    let scrollPosition = docElem.scrollTop;

    if (scrollPosition < 1) {
      const body = document.getElementsByName('body')[0];
      scrollPosition = body ? body.scrollTop : 0;
    }

    if (scrollPosition > this.previousScroll) {
      if ((scrollPosition + window.innerHeight) >= (document.documentElement.scrollHeight - 20)) {
        this.scrollSubject.next(scrollPosition);
      }
    }
  }

  ngOnInit() {
    this.searchService.setHidden(false);

    this.route.paramMap.subscribe(params => {
      this.tagName = params.get('tagName');
      console.log(`Received tag: ${this.tagName}`);

      if (this.tagName) {
        this.pageInfoService.setTitle(this.tagName);
        this.mode = 2;
      }
      else {
        this.pageInfoService.setTitle('Latest Photos');
        this.mode = 1;
      }

      this.loadPhotos();
    });

    this.searchSubscription = this.searchService.getKeywords()
      .subscribe(keywords => {
        if (keywords) {
          console.log(`Received search keywords: ${keywords}`);
          this.keywords = keywords;
          this.mode = 3;
        }
        else {
          this.keywords = null;
          this.mode = 1;
        }

        this.resetResults();
        this.loadPhotos();
      });

    this.organizeSubscription = this.organizeService.getEnabled()
      .subscribe(enabled => {
        this.organizeMode = enabled;
        this.clearSelections();
      });

    this.bottomScrollSubscription = this.scrollSubject
      .pipe(debounce(() => timer(1000)))
      .subscribe(() => {
        this.pageNum++;
        this.loadPhotos();
      });

    this.userSettingsSubscription = this.userSettingsService.getSettings()
      .subscribe(settings => {
        this.userSettings = settings;
        this.thumbHeight = this.getThumbHeight(settings.thumbnailSize);

        this.thumbnails.forEach((thumb) => {
          const ratio = thumb.thumbWidth / thumb.thumbHeight;
          const width = Math.floor(this.thumbHeight * ratio);

          thumb.thumbHeight = this.thumbHeight;
          thumb.thumbWidth = width;
        });
      });
  }

  ngOnDestroy() {
    this.searchSubscription.unsubscribe();
    this.organizeSubscription.unsubscribe();
    this.bottomScrollSubscription.unsubscribe();
  }

  select(thumbnail: Thumbnail) {
    if (this.organizeMode) {
      thumbnail.selected = !thumbnail.selected;
    }
    else {
      this.showLightbox(thumbnail);
    }
  }

  showLightbox(thumbnail: Thumbnail) {
    // https://github.com/blueimp/Gallery#lightbox-setup
    const options = {
      event: window.event,
      slideshowInterval: this.getSlideshowSpeed(this.userSettings.slideshowSpeed),
      startSlideshow: this.userSettings.autoStartSlideshow,
      fullScreen: false,
      thumbnailIndicators: true
    };

    const images: any[] = [];
    this.thumbnails.forEach(thumb => {
      images.push({
        href: `${thumb.thumbUrl}?type=full`,
        type: 'image/jpeg',
        thumbnail: thumb.thumbUrl
      });
    });

    const gallery = blueimp.Gallery(images, options);
    gallery.slide(this.thumbnails.findIndex(t => t.photoId ===  thumbnail.photoId), 0);
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

  private loadPhotos() {
    if (this.currentUser) {
      switch (this.mode) {
        case 1:
          this.photosService.getLatest(this.pageNum)
            .pipe(map(photos => this.photosToThumbnails(photos)))
            .subscribe((thumbs => this.appendThumbnails(thumbs)), this.handleLoadError);
          break;
        case 2:
          this.photosService.getPhotosByTag(this.pageNum, this.tagName)
            .pipe(map(photos => this.photosToThumbnails(photos)))
            .subscribe((thumbs => this.appendThumbnails(thumbs)), this.handleLoadError);
          break;
        case 3:
          this.photosService.searchPhotos(this.pageNum, this.keywords)
            .pipe(map(photos => this.photosToThumbnails(photos)))
            .subscribe((thumbs => this.appendThumbnails(thumbs)), this.handleLoadError);
        break;
      }
    }
  }

  private handleLoadError(error: any) {
    console.error(error);
    this.toastr.error('Failed to load photos');
  }

  private appendThumbnails(newThumbs: Thumbnail[]): void {
    newThumbs.forEach(t => this.thumbnails.push(t));
  }

  private resetResults(): void {
    this.pageNum = 1;
    this.thumbnails = [];
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
        const thumbsPerLine = Math.floor(windowWidth / (width + 8));
        const extra = windowWidth - (thumbsPerLine * (width + 8));
        extraPerThumb = Math.floor((extra - 8) / thumbsPerLine);
    }
    else {
        // display with normal image aspect ratio (no cropping)
        const ratio = photo.imageWidth / photo.imageHeight;
        width = Math.floor(height * ratio);
    }

    return { height: height + extraPerThumb, width: width + extraPerThumb };
  }

  private getThumbHeight(type: string): number {
    switch (type) {
      case 'Largest':
        return 200;
      case 'Large':
        return 150;
      case 'Medium':
        return 100;
      case 'Small':
        return 70;
      case 'Smallest':
        return 50;
      default:
        return 150;
    }
  }

  private getSlideshowSpeed(type): number {
    switch (type) {
      case 'Fastest':
        return 1500;
      case 'Fast':
        return 2750;
      case 'Normal':
        return 4000;
      case 'Slow':
        return 6000;
      case 'Slowest':
        return 9000;
      default:
        return 4000;
    }
  }
}
