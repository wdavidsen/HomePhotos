import { Component, OnInit } from '@angular/core';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { SettingsService } from '../services/settings.service';
import { Settings } from '../models/settings';
import { map } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.css']
})
export class SettingsComponent implements OnInit {
  settings: Settings;
  settingsForm: FormGroup;
  loading = false;
  submitted = false;

  constructor(
    private formBuilder: FormBuilder,
    private settingsService: SettingsService,
    private toastr: ToastrService) {
    }

  ngOnInit() {
    this.setupForm(new Settings());

    this.settingsService.getSettings()
      .pipe(map(settings => {
        this.settings = settings;
        return settings;
      }))
      .subscribe(
        data => {
          this.setupForm(data);
          this.loading = false;
        },
        error => {
          console.error(error);
          this.toastr.error('Failed to load settings');
          this.loading = false;
        });
  }

  onSubmit() {
    this.submitted = true;

    // stop here if form is invalid
    if (this.settingsForm.invalid) {
        return;
    }

    this.loading = true;
    const newSettings = this.formToSettings();
    this.settingsService.updateSettings(newSettings)
        .subscribe(
          data => {
            this.toastr.success('Successfully saved settings');
            this.settings = newSettings;
            this.loading = false;
          },
          error => {
            console.error(error);
            this.toastr.error('Failed to save settings');
            this.loading = false;
          });
  }

  // convenience getter for easy access to form fields
  get f() { return this.settingsForm.controls; }

  private setupForm(data: Settings) {

    this.settingsForm = this.formBuilder.group({
      indexPath: [data ? data.indexPath : '', Validators.required],
      cacheFolder: [data ? data.cacheFolder : '', Validators.required],
      mobileUploadsFolder: [data ? data.mobileUploadsFolder : '', Validators.required],
      nextIndexTime: [data ? data.nextIndexTime : '', Validators.required],
      indexFrequencyHours: [data ? data.indexFrequencyHours : '', Validators.required],
      largeImageSize: [data ? data.largeImageSize : '', Validators.required],
      smallImageSize: [data ? data.smallImageSize : '', Validators.required],
      thumbnailSize: [data ? data.thumbnailSize : '', Validators.required]});
  }

  private formToSettings(): Settings {
    const settings = new Settings ();

    settings.indexPath = this.f.indexPath.value;
    settings.cacheFolder = this.f.cacheFolder.value;
    settings.mobileUploadsFolder = this.f.mobileUploadsFolder.value;
    settings.nextIndexTime = this.f.nextIndexTime.value;
    settings.indexFrequencyHours = this.f.indexFrequencyHours.value;
    settings.smallImageSize = this.f.smallImageSize.value;
    settings.largeImageSize = this.f.largeImageSize.value;
    settings.thumbnailSize = this.f.thumbnailSize.value;

    return settings;
  }
}
