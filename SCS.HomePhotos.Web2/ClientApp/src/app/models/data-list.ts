import { LogEntry, PageInfo } from '.';

export class DataList  {

    constructor(data: LogEntry[], pageInfo: PageInfo) {
        this.data = data;
        this.pageInfo = pageInfo;
    }

    data: Array<object>;
    pageInfo: PageInfo;
}
