import { NO_ERRORS_SCHEMA } from '@angular/core';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { ToastrService } from 'ngx-toastr';
import { of } from 'rxjs';
import { DataList, LogEntry, PageInfo } from '../models';
import { AuthService } from '../services';
import { LogService } from '../services/log.service';
import { LogsComponent } from './logs.component';

describe('LogsComponent', () => {
    let component: LogsComponent;
    let fixture: ComponentFixture<LogsComponent>;
    let router;

    let mockToastr, mockLogService, mockAuthenticationService;

    beforeEach(async(() => {
        mockToastr = jasmine.createSpyObj(['error']);
        mockLogService = jasmine.createSpyObj(['getLatest']);
        mockAuthenticationService = jasmine.createSpyObj(['getCurrentUser', 'loadCsrfToken']);

        const logEntries: LogEntry[] = [
            { timestamp: new Date(), message: 'message 1', category: 'General', serverity: 'Neutral' },
            { timestamp: new Date(), message: 'message 2', category: 'Security', serverity: 'Critical' },
            { timestamp: new Date(), message: 'message 3', category: 'Index', serverity: 'Elevated' }
        ];
        const pageInfo: PageInfo = { pageNum: 1, pageSize: 15, sortBy: 'Timestamp', sortDescending: true, totalRecords: 0 };
        const dataList = new DataList(logEntries, pageInfo);

        mockLogService.getLatest.and.returnValue(of(dataList));
        mockAuthenticationService.loadCsrfToken.and.returnValue(of(true));

        TestBed.configureTestingModule({
            declarations: [LogsComponent],
            imports: [FormsModule, RouterTestingModule],
            providers: [
              { provide: LogService, useValue: mockLogService },
              { provide: ToastrService, useValue: mockToastr },
              { provide: AuthService, useValue: mockAuthenticationService }
            ],
            schemas: [NO_ERRORS_SCHEMA]
          })
          .compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(LogsComponent);
        component = fixture.componentInstance;

        router = TestBed.get(Router);
        // location = TestBed.get(Location);

        fixture.detectChanges();
        router.initialNavigation();
      });

      it('should create', () => {
        expect(component).toBeTruthy();
      });

      it('should initialize', () => {
        expect(component.logEntries.length).toBe(3);
        expect(component.filters.category).toBe('');
        expect(component.filters.severity).toBe('');
      });

      it('should filter', () => {
        mockLogService.getLatest.calls.reset();

        component.filtersChanged('');

        expect(component.pageInfo.pageNum).toBe(1);
        expect(mockLogService.getLatest).toHaveBeenCalledTimes(1);
      });

      it('should change page size', () => {
        mockLogService.getLatest.calls.reset();

        component.pageSizeChanged('');

        expect(component.pageInfo.pageNum).toBe(1);
        expect(mockLogService.getLatest).toHaveBeenCalledTimes(1);
      });

      it('should change page number', () => {
        mockLogService.getLatest.calls.reset();

        component.pageChanged({itemsPerPage: 15, page: 2});

        expect(component.pageInfo.pageNum).toBe(2);
        expect(mockLogService.getLatest).toHaveBeenCalledTimes(1);
      });

      it('should render log entries', () => {

        expect(fixture.nativeElement.querySelectorAll('table tbody tr').length).toBeGreaterThan(0);
      });
});
