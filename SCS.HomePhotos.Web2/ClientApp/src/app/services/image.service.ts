import { Injectable } from '@angular/core';

@Injectable()
export class ImageService {

    isValidImage(file: File): boolean {
        return /.+\.(png|jpg|jpeg)$/i.test(file.name);
    }

    blobToFile(blob: Blob, name: string): File {
        const temp: any = blob;
        temp.name = name;
        temp.lastModifiedDate = new Date();
        return temp;
    }

    resizeImage(file: File, maxWidth: number, maxHeight: number): Promise<Blob> {
        return new Promise((resolve, reject) => {
            const image = new Image();
            image.src = URL.createObjectURL(file);
            image.onload = () => {
                const width = image.width;
                const height = image.height;

                if (width <= maxWidth && height <= maxHeight) {
                    resolve(file);
                }

             let newWidth;
                let newHeight;

                if (width > height) {
                    newHeight = height * (maxWidth / width);
                    newWidth = maxWidth;
                }
                else {
                    newWidth = width * (maxHeight / height);
                    newHeight = maxHeight;
                }

                const canvas = document.createElement('canvas');
                canvas.width = newWidth;
                canvas.height = newHeight;

                const context = canvas.getContext('2d');

                context.drawImage(image, 0, 0, newWidth, newHeight);

                canvas.toBlob(resolve, file.type);
            };
            image.onerror = reject;
        });
    }
}
