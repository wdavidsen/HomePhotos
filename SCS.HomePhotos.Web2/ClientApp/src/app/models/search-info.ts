export class SearchInfo  {    
    fromDate?: Date;
    toDate?: Date;
    keywords?: string;
    changingContext?: boolean;

    static isSearchClear(serchInfo: SearchInfo): boolean {
        const info = serchInfo;
        if (!info) return true;
        if (info.keywords && info.keywords.length) return false;
        if (info.fromDate) return false;
        if (info.toDate) return false;
        return true;
      } 
}
