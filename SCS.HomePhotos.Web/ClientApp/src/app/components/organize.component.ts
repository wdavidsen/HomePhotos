import { Component, OnInit, OnDestroy } from '@angular/core';
import { OrganizeService } from '../services';

@Component({
    selector: 'app-organize',
    templateUrl: 'organize.component.html'
})
export class OrganizeComponent implements OnInit, OnDestroy {
    enabled = false;

    constructor(private organizeService: OrganizeService) { }

    ngOnInit() {
        this.organizeService.getEnabled()
            .subscribe(enabled => this.enabled = enabled);
    }

    ngOnDestroy() {

    }

    toggle() {
        this.enabled = !this.enabled;
        this.organizeService.setEnabled(this.enabled);
    }
}
