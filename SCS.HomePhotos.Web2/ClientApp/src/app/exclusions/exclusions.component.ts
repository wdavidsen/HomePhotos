import { Component, OnInit, OnDestroy } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { FileExclusion } from '../models/file-exclusion';
import { ExclusionService } from '../services/exclusion.service';

@Component({
  selector: 'app-exclusions',
  templateUrl: './exclusions.component.html',
  styleUrls: ['./exclusions.component.css']
})
export class ExclusionsComponent implements OnInit, OnDestroy {
    exclusions: Array<FileExclusion> = [];
    newPath: string = '';
    
    constructor(
        private exclusionService: ExclusionService,
        private toastr: ToastrService) {
    }

  ngOnInit() {   
    this.loadExclusions();
  }

  ngOnDestroy() {    
  }

  loadExclusions() {
    this.exclusionService.getExclusions()
        .subscribe({
            next: (exclusions) => this.exclusions = exclusions,
            error: (e) => { console.error(e); this.toastr.success('Failed to load exclusions'); }
        });
  }

  addExclusion() {
    if (this.newPath) {
        this.exclusionService.addExclusion({fullPath: this.newPath})
            .subscribe({
                next: () => { 
                    this.toastr.success('Exclusion added successfully'); 
                    this.loadExclusions(); 
                    this.newPath = '';
                },
                error: (e) => { console.error(e); this.toastr.success('Failed to add exclusion'); }
            });
    }
  }

  deleteExclusion(id: number) {
    this.exclusionService.deleteExclusion(id)
        .subscribe({
            next: () => {
                this.toastr.success('Deleted exclusion successfully');
                this.loadExclusions(); 
            },
            error: (e) => { console.error(e); this.toastr.error('Failed to delete exclusion'); }
        });
  }
}
