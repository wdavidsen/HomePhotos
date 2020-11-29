import { Injectable } from '@angular/core';
import { HttpResponse, HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable()
export class ProgressInterceptor implements HttpInterceptor {
  private requests: HttpRequest<any>[] = [];

  constructor(private spinner: NgxSpinnerService) { }

  removeRequest(req: HttpRequest<any>) {
    const i = this.requests.indexOf(req);

    if (i >= 0) {
      this.requests.splice(i, 1);
    }

    if (this.requests.length > 0) {
        this.spinner.show();
    }
    else {
        this.spinner.hide();
    }
  }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (req.url.indexOf('/photo-image') === -1 ) {
      this.requests.push(req);
      this.spinner.show();
    }

    return Observable.create(observer => {
      const subscription = next.handle(req)
        .subscribe(
          event => {
            if (event instanceof HttpResponse) {
              this.removeRequest(req);
              observer.next(event);
            }
          },
          err => {
            this.removeRequest(req);
            observer.error(err);
          },
          () => {
            this.removeRequest(req);
            observer.complete();
          });

      return () => {
        this.removeRequest(req);
        subscription.unsubscribe();
      };
    });
  }
}
