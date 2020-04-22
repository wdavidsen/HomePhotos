import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { TagState } from '../models';

@Component({
    selector: 'app-tri-check',
    template: `<div class="{{classes}}" (click)="toggle()"><div></div><label>{{label}}</label></div>`,
    styleUrls: ['./tri-check.component.css']
})
export class TriCheckComponent implements OnInit {
    classes = 'tricheck';

    @Input()
    label: string;

    @Input()
    checked: boolean;

    @Input()
    indeterminate: boolean;

    @Input()
    allowIndeterminate: boolean;

    @Output()
    changed = new EventEmitter<TriCheckState>();

    constructor() { }

    ngOnInit() {
        this.setClass(this.checked, this.indeterminate);
    }

    toggle() {
        if (this.indeterminate) {
            this.checked = !this.checked;
            this.indeterminate = false;
        }
        else
        {
            if (this.checked) {
                if (this.allowIndeterminate) {
                    this.indeterminate = true;
                }
                else {
                    this.checked = false;
                }
            }
            else {
                this.checked = true;
            }
        }

        this.setClass(this.checked, (this.allowIndeterminate && this.indeterminate));

        this.changed.emit({
            label: this.label,
            checked: this.checked,
            indeterminate: this.indeterminate
        });
    }

    private setClass(checked, indeterminate) {

        if (indeterminate) {
            this.classes = 'tricheck tricheck-indeterminate';
        }
        else if (checked) {
            this.classes = 'tricheck tricheck-on';
        }
        else {
            this.classes = 'tricheck';
        }
    }
}

export class TriCheckState {
    label: string;
    checked: boolean;
    indeterminate: boolean;
}
