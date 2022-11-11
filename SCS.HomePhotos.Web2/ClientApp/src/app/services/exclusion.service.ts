import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { FileExclusion } from '../models/file-exclusion';

@Injectable()
export class ExclusionService {

    constructor(private http: HttpClient) { }

    getExclusions(): Observable<FileExclusion[]> {
        return this.http.get<FileExclusion[]>(`${environment.apiUrl}/file-exclusions`);
    }

    addExclusion(exclusion: FileExclusion): Observable<FileExclusion> {
        return this.http.post<FileExclusion>(`${environment.apiUrl}/file-exclusions`, exclusion);
    }

    deleteTag(fileExclusionId: number): Observable<void> {
        return this.http.delete<void>(`${environment.apiUrl}/file-exclusions/${fileExclusionId}`);
    }
}
