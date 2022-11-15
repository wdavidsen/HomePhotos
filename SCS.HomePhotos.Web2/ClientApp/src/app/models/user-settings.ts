export class UserSettings {

  constructor() {
    this.thumbnailSize = 'Large';
    this.slideshowSpeed = 'Normal';
    this.autoStartSlideshow = false;
    this.defaultView = ''
  }

  thumbnailSize: string;
  slideshowSpeed: string;
  autoStartSlideshow: boolean;
  defaultView: string;
}
