import { HttpClientModule } from '@angular/common/http';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { ModalModule } from 'ngx-bootstrap/modal';
import { IndividualConfig, ToastrService } from 'ngx-toastr';

import { AccountComponent } from './account.component';

describe('AccountComponent', () => {
  let component: AccountComponent;
  let fixture: ComponentFixture<AccountComponent>;
  let mockToastr;

  beforeEach(async(() => {
    mockToastr = jasmine.createSpyObj(['success', 'error']);

    TestBed.configureTestingModule({
      declarations: [ AccountComponent ],
      imports: [ReactiveFormsModule, HttpClientModule, RouterTestingModule, ModalModule.forRoot()],
      providers: [
        { provide: ToastrService, useValue: mockToastr }
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
