import { Component, OnInit, OnDestroy } from '@angular/core';
import { Settings } from '../models';
import { ExclusionService } from '../services/exclusion.service';
import { SettingsService } from '../services/settings.service';

@Component({
  selector: 'app-exclusions',
  templateUrl: './exclusions.component.html',
  styleUrls: ['./exclusions.component.css']
})
export class ExclusionsComponent implements OnInit, OnDestroy {
    folders: Array<string> = [];
    newPath: string = '';
    private settings: Settings;

    constructor(
        private settingsService: SettingsService,
        private exclusionService: ExclusionService) {

    }

  ngOnInit() {
    this.settingsService.getSettings()
        .subscribe({
            next: s => {
                this.settings = s;
            }
        });

    this.folders = [
        'c:\\homephotos\\index\\folder1',
        'c:\\homephotos\\index\\folder2\\img-123.jpg'
    ];
  }

  ngOnDestroy() {
    
  }

  addExclusion() {
    if (this.newPath) {
        const newPath = this.newPath.replace('/', '\\');
        const isMobile = this.isMobilePath(newPath);
        const isNormal = this.isIndexPath(newPath);

        if (isMobile || isNormal) {
            const relativePath = this.removeRoot(newPath, isMobile);
            const fileName = relativePath.substring(relativePath.lastIndexOf('\\'));
            this.exclusionService.addExclusion({
                
            });
        }
    }
  }

  deleteExclusion(id: number) {

  }

  private isIndexPath(path: string): boolean {
    return path.toLowerCase().indexOf(this.settings.indexPath.toLocaleLowerCase()) == 0;
  }

  private isMobilePath(path: string): boolean {
    return path.toLowerCase().indexOf(this.settings.mobileUploadsFolder.toLocaleLowerCase()) == 0;
  }

  private removeRoot(path: string, isMobile: boolean): string {
    if (isMobile) {        
        return path.substring(this.settings.mobileUploadsFolder.length);
    }
    else {
        return path.substring(this.settings.indexPath.length);
    }
  }
}
