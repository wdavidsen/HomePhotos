import { OnInit, Component } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { UserSettingsService } from '../services';
import { ToastrService } from 'ngx-toastr';
import { UserSettings } from '../models/user-settings';

@Component({
    selector: 'app-user-settings',
    templateUrl: './user-settings.component.html',
    styleUrls: ['./user-settings.component.css']
  })

export class UserSettingsComponent implements OnInit {
    title: string;
    userSettings: UserSettings;

    constructor(
        public bsModalRef: BsModalRef,
        private userSettingsService: UserSettingsService,
        private toastr: ToastrService) {}

    ngOnInit() {

    }

    saveSettings() {
        // this.userSettingsService()
    }

    private settingsToForm() {

    }

    private formToSettings(): UserSettings {
        return null;
    }
}
