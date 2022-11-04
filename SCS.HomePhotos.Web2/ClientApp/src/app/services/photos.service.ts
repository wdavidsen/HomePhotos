import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { Photo } from '../models';

@Injectable()
export class PhotosService {

    constructor(private http: HttpClient) {

    }

    getLatest(pageNum: number): Observable<Photo[]> {
        return this.http.get<Photo[]>(`${environment.apiUrl}/photos/latest?pageNum=${pageNum}`);
    }

    getPhotosByTag(pageNum: number, tagName: string): Observable<Photo[]> {
        return this.http.get<Photo[]>(`${environment.apiUrl}/photos/byTag?pageNum=${pageNum}&tag=${escape(tagName)}`);
    }

    searchPhotos(pageNum: number, keywords: string) {
        return this.http.get<Photo[]>(`${environment.apiUrl}/photos/search?pageNum=${pageNum}&keywords=${escape(keywords)}`);
    }

    deletePhoto(photoId: number): Observable<void> {
        return this.http.delete<void>(`${environment.apiUrl}/photos/${photoId}`);
    }
}
