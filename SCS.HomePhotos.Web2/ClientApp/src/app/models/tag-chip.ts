import { Tag } from "./tag";

export class TagChip {
    // constructor(name: string, count: number, isDivider: boolean = false) {
    //     this.name = name;
    //     this.count = count;
    //     this.isDivider = isDivider;
    // }
    id?: number;
    name?: string;
    isDivider?: boolean;
    selected?: boolean;
    count?: number;
    color?: string;
    borderColor?: string;
    ownerId?: number;

    static toTag(chip: TagChip): Tag {
        return {
            tagId: chip.id,
            tagName: chip.name,
            ownerId: chip.ownerId
          };
    }

    static get defaultColor() { return 'rgb(255, 249, 196)'; }
}
