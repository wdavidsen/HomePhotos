import { Component, OnInit } from '@angular/core';
import { LogEntry, PageInfo } from '../models';
import { ToastrService } from 'ngx-toastr';
import { LogService } from '../services/log.service';
import { PageChangedEvent } from 'ngx-bootstrap/pagination/pagination.component';
import { tap } from 'rxjs/operators';

@Component({
  selector: 'app-logs',
  templateUrl: './logs.component.html',
  styleUrls: ['./logs.component.css']
})
export class LogsComponent implements OnInit {
  public logEntries: LogEntry[];
  public loading = false;

  filters = { category: '', severity: '' };

  totalEntries = 0;
  pageInfo: PageInfo = { pageNum: 1, pageSize: 15, sortBy: 'Timestamp', sortDescending: true, totalRecords: 0 };

  constructor(
    private logService: LogService,
    private toastr: ToastrService) {
    }

  filtersChanged(event: any): void {
    this.loadEntries();
  }

  pageSizeChanged(event: any): void {
    this.loadEntries();
  }

  pageChanged(event: PageChangedEvent): void {
    this.pageInfo.pageNum = event.page;
    this.loadEntries();
  }

  ngOnInit() {
    this.loadEntries();
  }

  private loadEntries(): void {
    this.logService.getLatest(this.filters.category, this.filters.severity, this.pageInfo)
      .pipe(tap(result => this.pageInfo = result.pageInfo))
      .subscribe(
        entries => this.logEntries = <LogEntry[]>entries.data,
        error => {
          console.error(error);
          this.toastr.error('Failed to load activity');
        });
  }
}
