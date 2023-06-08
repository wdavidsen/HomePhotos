import { OnInit, Component } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AuthService, TagService, UserSettingsService } from '../services';
import { ToastrService } from 'ngx-toastr';
import { UserSettings } from '../models/user-settings';
import { UntypedFormGroup, UntypedFormBuilder, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { User } from 'oidc-client';

@Component({
  selector: 'app-user-settings',
  templateUrl: './user-settings.component.html',
  styleUrls: ['./user-settings.component.css']
})

export class UserSettingsComponent implements OnInit {
  title: string;
  userSettings: UserSettings;
  userSettingsForm: UntypedFormGroup;
  loading = false;
  submitted = false;
  thumbSizes = ['Largest', 'Large', 'Medium', 'Small', 'Smallest'];
  slideSpeeds = ['Fastest', 'Fast', 'Normal', 'Slow', 'Slowest'];
  userScopes = [    
    { text: 'Show shared and personal photos and tags', value: 'SharedAndPersonal' },
    { text: 'Show personal photos and tags only', value: 'PersonalOnly' }];
  defaultTags: Array<string> = [];

  constructor(
    private formBuilder: UntypedFormBuilder,
    public bsModalRef: BsModalRef,
    private userSettingsService: UserSettingsService,
    private tagService: TagService,
    private toastr: ToastrService,
    private authenticationService: AuthService) {
      this.authenticationService.getCurrentUser().subscribe(user => {
        if (user.role === 'Admin') {
          this.userScopes.splice(0, 0, { text: 'Show all photos and tags', value: 'Everything' })
        }
      });
    }

  ngOnInit() {
    this.userSettings = this.userSettingsService.userSettings;
    this.setupForm(this.userSettings);

    this.tagService.getTags()
      .subscribe({
        next: (tags) => {
          let options = tags.sort((a, b) => {
            if (a.ownerId ?? 99 < b.ownerId ?? 99) { return -1; }
            if (a.ownerId ?? 99> b.ownerId ?? 99) { return 1; }
            return 0;
          })
          .map((t) => `${t.tagName} (${t.ownerUsername ?? 'Shared'})`);
                    
          this.defaultTags = options;
        },
        error: (response: HttpErrorResponse) => console.error(response)
      })
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
      displayScope: [data ? data.userScope : 'Everything', Validators.required],
      thumbnailSize: [data ? data.thumbnailSize : 'Medium', Validators.required],
      slideshowSpeed: [data ? data.slideshowSpeed : 'Normal', Validators.required],
      autoStartSlideshow: [data ? data.autoStartSlideshow : false, Validators.required],
      defaultView: [data ? data.defaultView : '']
    });
  }

  private formToUserSettings(): UserSettings {
    const settings = new UserSettings();

    settings.userScope = this.f.displayScope.value;
    settings.thumbnailSize = this.f.thumbnailSize.value;
    settings.slideshowSpeed = this.f.slideshowSpeed.value;
    settings.autoStartSlideshow = this.f.autoStartSlideshow.value;
    settings.defaultView = this.f.defaultView.value;
    
    return settings;
  }
}
