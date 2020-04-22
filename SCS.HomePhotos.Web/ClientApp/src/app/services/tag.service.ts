import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { Tag, TagState } from '../models';
import { map } from 'rxjs/operators';

@Injectable()
export class TagService {

    constructor(private http: HttpClient) {

    }

    getTags(): Observable<Tag[]> {
        return this.http.get<Tag[]>(`${environment.apiUrl}/tags`);
    }

    searchTags(keywords: string): Observable<Tag[]> {
        return this.http.get<Tag[]>(`${environment.apiUrl}/tags/search?keywords=${escape(keywords)}`);
    }

    getPhototags(photoIds: number[]): Observable<TagState[]> {
        return this.http.post<TagState[]>(`${environment.apiUrl}/tags/batchTag`, photoIds)
            .pipe(map(data => data['tags']));
    }

    updatePhotoTags(photoIds: number[], tagStates: TagState[]): Observable<any> {
        const payload = {
            photoIds: photoIds,
            tagStates: tagStates
        };
        return this.http.put(`${environment.apiUrl}/tags/batchTag`, payload);
    }
}
