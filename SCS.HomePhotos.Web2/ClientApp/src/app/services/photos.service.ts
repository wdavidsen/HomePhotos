import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { Photo, SearchInfo } from '../models';

@Injectable()
export class PhotosService {

    constructor(private http: HttpClient) {

    }

    getLatest(pageNum: number): Observable<Photo[]> {
        return this.http.get<Photo[]>(`${environment.apiUrl}/photos/latest?pageNum=${pageNum}`);
    }

    getPhotosByTag(pageNum: number, tagName: string, owner: string|null): Observable<Photo[]> {
        return this.http.get<Photo[]>(`${environment.apiUrl}/photos/byTag?pageNum=${pageNum}&tag=${encodeURIComponent(tagName)}&owner=${owner ?? ''}`);
    }

    searchPhotos(pageNum: number, searchInfo: SearchInfo) {
        const keywords = searchInfo.keywords ? encodeURIComponent(searchInfo.keywords) : '';
        return this.http.get<Photo[]>(`${environment.apiUrl}/photos/search?pageNum=${pageNum}&keywords=${keywords}&fromDate=${searchInfo.fromDate ?? ''}&toDate=${searchInfo.toDate ?? ''}`);
    }

    deletePhoto(photoId: number): Observable<void> {
        return this.http.delete<void>(`${environment.apiUrl}/photos/${photoId}`);
    }
}
