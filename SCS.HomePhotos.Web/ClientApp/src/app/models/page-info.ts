export class PageInfo  {
    constructor() {
        this.pageNum = 1;
        this.pageSize = 50;
        this.sortBy = null;
        this.sortDescending = false;
        this.totalRecords = 0;
    }

    pageNum: number | null;
    pageSize: number | null;
    sortBy: string | null;
    sortDescending: boolean | null;
    totalRecords: number | null;
}
