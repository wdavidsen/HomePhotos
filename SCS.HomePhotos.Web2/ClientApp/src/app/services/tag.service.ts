import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { SearchInfo, Tag, TagState } from '../models';
import { map } from 'rxjs/operators';

@Injectable()
export class TagService {
    constructor(private http: HttpClient) {}

    getTags(userName?: string): Observable<Tag[]> {
        const url = userName ? `${environment.apiUrl}/tags/${userName}`: `${environment.apiUrl}/tags`;
        return this.http.get<Tag[]>(url);
    }

    searchTags(searchInfo: SearchInfo): Observable<Tag[]> {
        const keywords = searchInfo.keywords ? encodeURIComponent(searchInfo.keywords) : '';
        return this.http.get<Tag[]>(`${environment.apiUrl}/tags/search?keywords=${keywords}&fromDate=${searchInfo.fromDate ?? ''}&toDate=${searchInfo.toDate ?? ''}`);
    }

    addTag(tag: Tag): Observable<Tag> {
        return this.http.post<Tag>(`${environment.apiUrl}/tags`, tag);
    }

    updateTag(tag: Tag): Observable<Tag> {
        return this.http.put<Tag>(`${environment.apiUrl}/tags`, tag);
    }

    deleteTag(tagId: number): Observable<void> {
        return this.http.delete<void>(`${environment.apiUrl}/tags/${tagId}`);
    }

    copyTag(sourceTagId: number, newTagName: string, ownerId: number|null): Observable<Tag> {
        return this.http.put<Tag>(`${environment.apiUrl}/tags/copy`, { SourceTagId: sourceTagId, newTagName: newTagName, OwnerId: ownerId });
    }

    mergeTags(sourceTagIds: Array<number>, newTagName: string, ownerId: number|null): Observable<Tag> {
        return this.http.put<Tag>(`${environment.apiUrl}/tags/merge`, { sourceTagIds: sourceTagIds, newTagName: newTagName, OwnerId: ownerId });
    }

    getPhototags(photoIds: number[], userName?: string): Observable<TagState[]> {
        const url = userName ? `${environment.apiUrl}/tags/${userName}/batchTag`: `${environment.apiUrl}/tags/batchTag`;
        return this.http.post<TagState[]>(url, photoIds)
            .pipe(map(data => data['tags']));
    }

    updatePhotoTags(photoIds: number[], tagStates: TagState[], userName?: string): Observable<any> {
        const url = userName ? `${environment.apiUrl}/tags/${userName}/batchTag`: `${environment.apiUrl}/tags/batchTag`;
        const payload = {
            photoIds: photoIds,
            tagStates: tagStates
        };
        return this.http.put(url, payload);
    }
}
