import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { Photo } from '../models';

@Injectable()
export class PhotosService {

    constructor(private http: HttpClient) {

    }

    getLatest(): Observable<Photo[]> {
        return this.http.get<Photo[]>(`${environment.apiUrl}/photos/latest`);
    }

    getPhotosByTag(tagName: string): Observable<Photo[]> {
        return this.http.get<Photo[]>(`${environment.apiUrl}/photos/byTag?tag=${escape(tagName)}`);
    }

    searchPhotos(keywords: string) {
        return this.http.get<Photo[]>(`${environment.apiUrl}/photos/search?keywords=${escape(keywords)}`);
    }
}
