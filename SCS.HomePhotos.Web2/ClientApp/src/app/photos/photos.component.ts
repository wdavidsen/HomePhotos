import { Component, OnInit, OnDestroy, HostListener } from '@angular/core';
import { PhotosService } from '../services/photos.service';
import { Thumbnail, Photo, User } from '../models';
import { map, debounce } from 'rxjs/operators';
import { ActivatedRoute } from '@angular/router';
import { SearchService, OrganizeService, AuthService, UserSettingsService } from '../services';
import { PageInfoService } from '../services/page-info.service';
import { Subscription, Subject, timer } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { PhotoTaggerComponent } from './photo-tagger.component';
import { UserSettings } from '../models/user-settings';
import { environment } from 'src/environments/environment';
import { AlertDialogComponent, ConfirmDialogComponent } from '../common-dialog';

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
  confirmDeleteModalRef: BsModalRef;
  deleteCheckModalRef: BsModalRef;
  keywords: string;

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
    private authenticationService: AuthService) {      
      this.authenticationService.getCurrentUser().subscribe(user => {
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
        if (!this.keywords && !keywords) {
          return;
        }

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
          const dims = this.calculateThumbSize(ratio, this.thumbHeight);

          thumb.thumbHeight = dims.height;
          thumb.thumbWidth = dims.width;
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
    // const options = {
    //   container: '#blueimp-gallery',
    //   event: window.event,
    //   slideshowInterval: this.getSlideshowSpeed(this.userSettings.slideshowSpeed),
    //   startSlideshow: this.userSettings.autoStartSlideshow,
    //   fullScreen: false,
    //   thumbnailIndicators: false
    // };
    const startIndex = this.thumbnails.findIndex(t => t.photoId ===  thumbnail.photoId);

    const options = {      
      // The Id, element or querySelector of the gallery widget:
      container: '#blueimp-gallery',
      // The tag name, Id, element or querySelector of the slides container:
      slidesContainer: 'div',
      // The tag name, Id, element or querySelector of the title element:
      titleElement: 'h3',
      // The class to add when the gallery is visible:
      displayClass: 'blueimp-gallery-display',
      // The class to add when the gallery controls are visible:
      controlsClass: 'blueimp-gallery-controls',
      // The class to add when the gallery only displays one element:
      singleClass: 'blueimp-gallery-single',
      // The class to add when the left edge has been reached:
      leftEdgeClass: 'blueimp-gallery-left',
      // The class to add when the right edge has been reached:
      rightEdgeClass: 'blueimp-gallery-right',
      // The class to add when the automatic slideshow is active:
      playingClass: 'blueimp-gallery-playing',
      // The class to add when the browser supports SVG as img (or background):
      svgasimgClass: 'blueimp-gallery-svgasimg',
      // The class to add when the browser supports SMIL (animated SVGs):
      smilClass: 'blueimp-gallery-smil',
      // The class for all slides:
      slideClass: 'slide',
      // The slide class for the active (current index) slide:
      slideActiveClass: 'slide-active',
      // The slide class for the previous (before current index) slide:
      slidePrevClass: 'slide-prev',
      // The slide class for the next (after current index) slide:
      slideNextClass: 'slide-next',
      // The slide class for loading elements:
      slideLoadingClass: 'slide-loading',
      // The slide class for elements that failed to load:
      slideErrorClass: 'slide-error',
      // The class for the content element loaded into each slide:
      slideContentClass: 'slide-content',
      // The class for the "toggle" control:
      toggleClass: 'toggle',
      // The class for the "prev" control:
      prevClass: 'prev',
      // The class for the "next" control:
      nextClass: 'next',
      // The class for the "close" control:
      closeClass: 'close',
      // The class for the "play-pause" toggle control:
      playPauseClass: 'play-pause',
      // The list object property (or data attribute) with the object type:
      typeProperty: 'type',
      // The list object property (or data attribute) with the object title:
      titleProperty: 'title',
      // The list object property (or data attribute) with the object alt text:
      altTextProperty: 'alt',
      // The list object property (or data attribute) with the object URL:
      urlProperty: 'href',
      // The list object property (or data attribute) with the object srcset:
      srcsetProperty: 'srcset',
      // The list object property (or data attribute) with the object sizes:
      sizesProperty: 'sizes',
      // The list object property (or data attribute) with the object sources:
      sourcesProperty: 'sources',
      // The gallery listens for transitionend events before triggering the
      // opened and closed events, unless the following option is set to false:
      displayTransition: true,
      // Defines if the gallery slides are cleared from the gallery modal,
      // or reused for the next gallery initialization:
      clearSlides: true,
      // Toggle the controls on pressing the Enter key:
      toggleControlsOnEnter: true,
      // Toggle the controls on slide click:
      toggleControlsOnSlideClick: true,
      // Toggle the automatic slideshow interval on pressing the Space key:
      toggleSlideshowOnSpace: true,
      // Navigate the gallery by pressing the ArrowLeft and ArrowRight keys:
      enableKeyboardNavigation: true,
      // Close the gallery on pressing the Escape key:
      closeOnEscape: true,
      // Close the gallery when clicking on an empty slide area:
      closeOnSlideClick: true,
      // Close the gallery by swiping up or down:
      closeOnSwipeUpOrDown: false,
      // Close the gallery when the URL hash changes:
      closeOnHashChange: true,
      // Emulate touch events on mouse-pointer devices such as desktop browsers:
      emulateTouchEvents: true,
      // Stop touch events from bubbling up to ancestor elements of the Gallery:
      stopTouchEventsPropagation: false,
      // Hide the page scrollbars:
      hidePageScrollbars: true,
      // Stops any touches on the container from scrolling the page:
      disableScroll: true,
      // Carousel mode (shortcut for carousel specific options):
      carousel: false,
      // Allow continuous navigation, moving from last to first
      // and from first to last slide:
      continuous: true,
      // Remove elements outside of the preload range from the DOM:
      unloadElements: true,
      // Start with the automatic slideshow:
      startSlideshow: this.userSettings.autoStartSlideshow,
      // Delay in milliseconds between slides for the automatic slideshow:
      slideshowInterval: this.getSlideshowSpeed(this.userSettings.slideshowSpeed),
      // The direction the slides are moving: ltr=LeftToRight or rtl=RightToLeft
      slideshowDirection: 'ltr',
      // The starting index as integer.
      // Can also be an object of the given list,
      // or an equal object with the same url property:
      index: startIndex,
      // The number of elements to load around the current index:
      preloadRange: 2,
      // The transition duration between slide changes in milliseconds:
      transitionDuration: 300,
      // The transition duration for automatic slide changes, set to an integer
      // greater 0 to override the default transition duration:
      slideshowTransitionDuration: 500,
      // The event object for which the default action will be canceled
      // on Gallery initialization (e.g. the click event to open the Gallery):
      event: undefined,
      // Callback function executed when the Gallery is initialized.
      // Is called with the gallery instance as "this" object:
      onopen: undefined,
      // Callback function executed when the Gallery has been initialized
      // and the initialization transition has been completed.
      // Is called with the gallery instance as "this" object:
      onopened: undefined,
      // Callback function executed on slide change.
      // Is called with the gallery instance as "this" object and the
      // current index and slide as arguments:
      onslide: undefined,
      // Callback function executed after the slide change transition.
      // Is called with the gallery instance as "this" object and the
      // current index and slide as arguments:
      onslideend: undefined,
      // Callback function executed on slide content load.
      // Is called with the gallery instance as "this" object and the
      // slide index and slide element as arguments:
      onslidecomplete: undefined,
      // Callback function executed when the Gallery is about to be closed.
      // Is called with the gallery instance as "this" object:
      onclose: undefined,
      // Callback function executed when the Gallery has been closed
      // and the closing transition has been completed.
      // Is called with the gallery instance as "this" object:
      onclosed: undefined
    }

    const images: any[] = [];
    this.thumbnails.forEach(thumb => {
      images.push({
        href: `${thumb.thumbUrl}?type=full`,
        type: 'image/jpeg',
        thumbnail: thumb.thumbUrl
      });
    });

    const gallery = blueimp.Gallery(images, options);
    gallery.slide(this.thumbnails.findIndex(t => t.photoId ===  thumbnail.photoId));
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
      const options = {
        class: 'photo-tagger-dialog',
        initialState: {
          title: 'Photo Tagger',
          photoIds: selections.map(s => s.photoId)
        }
      };

      this.taggerModalRef = this.modalService.show(PhotoTaggerComponent, options);
    }
  }

  deletePhotos() {
    const selections = this.getSelectedThumbnails();

    if (selections.length > 10) {
      const checkOptions = AlertDialogComponent.GetOptions('Delete Limit Exceeded', 'You may only delete 10 photos at a time as a safety precaution. To remove photos in bulk, please remove photos from index folder.');
      this.deleteCheckModalRef = this.modalService.show(AlertDialogComponent, checkOptions);
      return;
    }

    const message = 'Are you sure you want to delete these photos?';
    const confirmOptions = ConfirmDialogComponent.GetOptions('confirm-resize-dialog', 'Delete Photos', message, false);
    this.confirmDeleteModalRef = this.modalService.show(ConfirmDialogComponent, confirmOptions);

    var sub = this.modalService.onHidden
      .subscribe(() => {
        if (this.confirmDeleteModalRef.content.yesClicked) {
          selections.forEach(t => {
            const photoId = t.photoId;

            this.photosService.deletePhoto(photoId)
                .subscribe({
                  next: () => {
                    const index = this.thumbnails.map(t => t.photoId).indexOf(photoId);
                    this.thumbnails.splice(index, 1);
                    this.toastr.success('Photo deleted successfully');
                  },
                  error: (e) => {
                    console.error(`Failed to delete photo ${photoId}. ${e.message}`);
                    this.toastr.error(`Failed to delete photo`)
                  }
                });
          });           
        }
        sub.unsubscribe();
      });2
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
    const msg = 'Failed to load photos';

    try {
      this.toastr.error(msg);
      console.error(error);
    }
    catch (e) {
      console.error(msg);
    }
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
    const ratio = photo.imageWidth / photo.imageHeight;
    const heightWidth = this.calculateThumbSize(ratio, this.thumbHeight);

    thumb.photoId = photo.photoId;
    thumb.selected = false;
    thumb.thumbUrl = `${environment.serverUrl}/photo-image/${photo.cacheFolder}/${photo.fileName}`;
    thumb.thumbHeight = heightWidth.height;
    thumb.thumbWidth = heightWidth.width;

    return thumb;
  }

  private calculateThumbSize(ratio: number, height: number) {
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
