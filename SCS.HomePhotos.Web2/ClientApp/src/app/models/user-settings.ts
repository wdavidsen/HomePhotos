export class UserSettings {

  constructor() {
    this.thumbnailSize = 'Large';
    this.slideshowSpeed = 'Normal';
    this.autoStartSlideshow = false;
    this.defaultView = null
  }

  thumbnailSize: string;
  slideshowSpeed: string;
  autoStartSlideshow: boolean;
  defaultView: string;

  static parseDefaultView(defaultView: string) : Array<string> {
    const tagName = defaultView.substring(0, defaultView.indexOf('(') -1 );
    let username = defaultView.match(/\((.+)\)/)[1];
    
    if (username.toUpperCase() == 'SHARED') {
      username = null;
    }
    return [username, tagName];
  }
}
