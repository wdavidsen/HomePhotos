import { Pipe, PipeTransform } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DomSanitizer } from '@angular/platform-browser';

@Pipe({
  name: 'secure'
})
export class SecurePipe implements PipeTransform {

  constructor(private http: HttpClient, private sanitizer: DomSanitizer) { }

  transform(url: string) {

    const pipe = this;

    return new Observable<any>((observer) => {
      // initial blank image
      observer.next(this.sanitizer.bypassSecurityTrustUrl('data:image/gif;base64,R0lGODlhAQABAAAAACH5BAEKAAEALAAAAAABAAEAAAICTAEAOw=='));

      const {next, error} = observer;

      this.http.get(url, {responseType: 'blob'}).subscribe(response => {
        const reader = new FileReader();
        reader.readAsDataURL(response);
        reader.onloadend = () => {
          observer.next(pipe.sanitizer.bypassSecurityTrustUrl(<string>reader.result));
        };
      });

      return {unsubscribe() {  }};
    });
  }
}
