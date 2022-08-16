import { OnInit, Component } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { UserSettingsService } from '../services';
import { ToastrService } from 'ngx-toastr';
import { UserSettings } from '../models/user-settings';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-user-settings',
  templateUrl: './user-settings.component.html',
  styleUrls: ['./user-settings.component.css']
})

export class UserSettingsComponent implements OnInit {
  title: string;
  userSettings: UserSettings;
  userSettingsForm: FormGroup;
  loading = false;
  submitted = false;
  thumbSizes = ['Largest', 'Large', 'Medium', 'Small', 'Smallest'];
  slideSpeeds = ['Fastest', 'Fast', 'Normal', 'Slow', 'Slowest'];

  constructor(
    private formBuilder: FormBuilder,
    public bsModalRef: BsModalRef,
    private userSettingsService: UserSettingsService,
    private toastr: ToastrService) {}

  ngOnInit() {
    this.userSettings = this.userSettingsService.userSettings;
    this.setupForm(this.userSettings);
  }

  onSubmit() {
    this.submitted = true;

    // stop here if form is invalid
    if (this.userSettingsForm.invalid) {
      return;
    }

    this.save();
  }

  private save() {
    const settings = this.formToUserSettings();
    this.userSettingsService.saveSettings(settings);

    this.toastr.success('Successfully saved your settings');
    this.bsModalRef.hide();
  }

  // convenience getter for easy access to form fields
  get f() { return this.userSettingsForm.controls; }

  private setupForm(data: UserSettings) {

    this.userSettingsForm = this.formBuilder.group({
      thumbnailSize: [data ? data.thumbnailSize : 'Medium', Validators.required],
      slideshowSpeed: [data ? data.slideshowSpeed : 'Normal', Validators.required],
      autoStartSlideshow: [data ? data.autoStartSlideshow : false, Validators.required]
    });
  }

  private formToUserSettings(): UserSettings {
    const settings = new UserSettings();

    settings.thumbnailSize = this.f.thumbnailSize.value;
    settings.slideshowSpeed = this.f.slideshowSpeed.value;
    settings.autoStartSlideshow = this.f.autoStartSlideshow.value;

    return settings;
  }
}
