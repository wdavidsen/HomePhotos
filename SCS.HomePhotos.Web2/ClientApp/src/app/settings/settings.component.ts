import { Component, OnInit, ViewChild } from '@angular/core';
import { UntypedFormGroup, Validators, UntypedFormBuilder } from '@angular/forms';
import { SettingsService } from '../services/settings.service';
import { Settings } from '../models/settings';
import { map } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';
import { BsModalService, BsModalRef, ModalDirective } from 'ngx-bootstrap/modal';
import { ConfirmDialogComponent } from '../common-dialog';
import * as moment from 'moment';
import { AuthService } from '../services';

declare var RGB_Log_Shade: any;

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.css']
})
export class SettingsComponent implements OnInit {
  @ViewChild('indexModal', {static: true})
  indexModal: ModalDirective;

  settings: Settings;
  settingsForm: UntypedFormGroup;
  loading = false;
  submitted = false;
  tagColor = 'rgb(255,249,196)';
  indexModalData: any = {
    allOrNew: 'NEW'
  };

  confirmModalRef: BsModalRef;

  constructor(
    private formBuilder: UntypedFormBuilder,
    private settingsService: SettingsService,
    private toastr: ToastrService,
    private modalService: BsModalService) {
    }

  ngOnInit() {
    this.setupForm(new Settings());

    this.settingsService.getSettings()
      .pipe(map(settings => {
        this.settings = settings;
        return settings;
      }))
      .subscribe({
        next: (data) => {
          this.setupForm(data);
          this.loading = false;
        },
        error: (e) => {
          console.error(e);
          this.toastr.error('Failed to load settings');
          this.loading = false;
        }
      });
  }

  onSubmit() {
    this.submitted = true;

    // stop here if form is invalid
    if (this.settingsForm.invalid) {
      return;
    }

    if (this.f.smallImageSize.dirty || this.f.largeImageSize.dirty || this.f.thumbnailSize.dirty)
    {
      const message = 'Would you like to update image sizes on the existing cache images during next index? If no, these settings will only apply to new photos added.';
      const options = ConfirmDialogComponent.GetOptions('confirm-resize-dialog', 'Existing Image Size', message, true);
      this.confirmModalRef = this.modalService.show(ConfirmDialogComponent, options);

      this.modalService.onHidden
        .subscribe(() => {
          if (this.confirmModalRef.content.cancelClicked) {
            return;
          }
          this.save(this.confirmModalRef.content.yesClicked);
        });
    }
    else {
      this.save(false);
    }
  }

  private save(reprocessPhotos: boolean) {
    this.loading = true;
    const newSettings = this.formToSettings();

    // console.log(newSettings.serverTagColor);

    // const borderColor = RGB_Log_Shade(-.7, newSettings.serverTagColor);
    // console.log(borderColor);
    //new RGB_Log_Shade()

    this.settingsService.updateSettings(newSettings, reprocessPhotos)
      .subscribe({
        next: (data) => {
          this.toastr.success('Successfully saved settings');
          this.settings = newSettings;
          this.loading = false;
        },
        error: (e) => {
          console.error(e);
          this.toastr.error('Failed to save settings');
          this.loading = false;
        }
      });
  }

  promptForIndex() {
    this.indexModal.show();
  }

  index() {
    this.settingsService.indexNow(this.indexModalData.allOrNew === 'ALL')
      .subscribe({
        next: (updatedSettings) => {
          this.f.nextIndexTime_date.setValue(moment(updatedSettings.nextIndexTime).format('YYYY-MM-DD'));
          this.f.nextIndexTime_time.setValue(moment(updatedSettings.nextIndexTime).format('HH:mm'));

          this.toastr.success('Index triggered successfully')
        },
        error: (e) => { console.error(e); this.toastr.error('Failed to trigger index') }
      });
      this.indexModal.hide();
  }

  cancelIndex() {
    this.indexModal.hide();
  }

  promptForClear() {
    const message = 'Are you sure you want to clear the photo cache? This action may take several minutes to complete.';
    const options = ConfirmDialogComponent.GetOptions('confirm-clear-dialog', 'Clear Cache', message, true);
    this.confirmModalRef = this.modalService.show(ConfirmDialogComponent, options);

    this.modalService.onHidden
      .subscribe(() => {
        if (this.confirmModalRef.content.yesClicked) {
          this.clear();
        }
      });
  }

  clear() {
    this.settingsService.clearCache()
      .subscribe({
        next: () => this.toastr.success('Cached cleared successfully'),
        error: (e) => { console.error(e); this.toastr.error('Failed to clear cache') }
      });
      this.indexModal.hide();
  }

   // convenience getter for easy access to form fields
  get f() { return this.settingsForm.controls; }

  private setupForm(data: Settings) { 
    this.settingsForm = this.formBuilder.group({
      indexPath: [data ? data.indexPath : '', Validators.required],
      cacheFolder: [data ? data.cacheFolder : '', Validators.required],
      mobileUploadsFolder: [data ? data.mobileUploadsFolder : '', Validators.required],
      nextIndexTime_date: [data && data.nextIndexTime ? moment(data.nextIndexTime).format('YYYY-MM-DD') : ''],
      nextIndexTime_time: [data && data.nextIndexTime ? moment(data.nextIndexTime).format('HH:mm') : ''],
      indexFrequencyHours: [data ? data.indexFrequencyHours : '', Validators.required],
      largeImageSize: [data ? data.largeImageSize : '', Validators.required],
      smallImageSize: [data ? data.smallImageSize : '', Validators.required],
      thumbnailSize: [data ? data.thumbnailSize : '', Validators.required],
      photoDeleteAction: [data ? data.photoDeleteAction : 0],
      mobilePhotoDeleteAction: [data ? data.mobilePhotoDeleteAction : 1],
      tagColor: [data ?  data.tagColor : 'rgb(255, 249, 196)']
    });
  }

  private formToSettings(): Settings {
    const settings = new Settings ();

    settings.indexPath = this.f.indexPath.value;
    settings.cacheFolder = this.f.cacheFolder.value;
    settings.mobileUploadsFolder = this.f.mobileUploadsFolder.value;
    settings.nextIndexTime = moment(this.f.nextIndexTime_date.value + ' '  + this.f.nextIndexTime_time.value).toDate();
    settings.indexFrequencyHours = this.f.indexFrequencyHours.value;
    settings.smallImageSize = this.f.smallImageSize.value;
    settings.largeImageSize = this.f.largeImageSize.value;
    settings.thumbnailSize = this.f.thumbnailSize.value;
    settings.photoDeleteAction = this.f.photoDeleteAction.value;
    settings.mobilePhotoDeleteAction = this.f.mobilePhotoDeleteAction.value;
    settings.tagColor = this.f.tagColor.value;

    return settings;
  }
}
