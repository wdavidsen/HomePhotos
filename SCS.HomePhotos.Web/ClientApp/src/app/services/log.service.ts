import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { PageInfo, DataList } from '../models';

@Injectable()
export class LogService {

    constructor(private http: HttpClient) {

    }

    getLatest(category: string, severity: string, pageInfo: PageInfo)
        : Observable<DataList> {

        const additionalParams = this.buildQuery(pageInfo);

        return this.http.get<DataList>(`${environment.apiUrl}/logs/latest?category=${category}&severity=${severity}${additionalParams}`);
    }

    private buildQuery(options: object) {
        let query = '';

        for (const p in options) {
            if (options[p] !== null) {
                query += `&${p}=${options[p]} `;
            }
        }
        return query.trim();
    }
}

