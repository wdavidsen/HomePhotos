import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { Tag } from '../models';

@Injectable()
export class TagService {

    constructor(private http: HttpClient) {

    }

    getTags(): Observable<Tag[]> {
        return this.http.get<Tag[]>(`${environment.apiUrl}/tags`);
    }

    searchTags(keywords: string) {
        return this.http.get<Tag[]>(`${environment.apiUrl}/tags/search?keywords=${escape(keywords)}`);
    }
}
