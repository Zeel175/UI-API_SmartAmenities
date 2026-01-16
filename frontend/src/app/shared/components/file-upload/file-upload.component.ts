import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Observable } from 'rxjs';

@Component({
  selector: "app-file-upload",
  templateUrl: './file-upload.component.html',
  styleUrl: './file-upload.component.scss'
})
export class FileUploadComponent {
  @Input() acceptTypes: string = '*/*'; // Default accepts all file types
  @Input() multiple: boolean = false; // Whether multiple files are allowed
  @Output() fileUploaded = new EventEmitter<any>(); // Emit event when file is uploaded

  selectedFile: File | null = null;
  uploadProgress: any = -1; // -1 means not uploading yet

  onFileSelect(event: any): void {
    this.selectedFile = event.target.files[0];
    this.onUpload();
  }

  onUpload(): void {
    if (!this.selectedFile) return;

    const formData = new FormData();
    formData.append('file', this.selectedFile, this.selectedFile.name);

    // Here you can integrate your service for file upload.
    // Example upload method
    this.uploadFile(formData).subscribe(
      (progress) => {
        this.uploadProgress = progress;
      },
      (error) => {
        console.error('Upload failed', error);
        this.uploadProgress = -1;
      },
      () => {
        this.fileUploaded.emit(this.selectedFile); // Emit the uploaded file when successful
      }
    );
  }

  // Method to simulate file upload (replace with your actual service call)
  uploadFile(formData: FormData) {
    // Simulate an upload process with observable
    return new Observable((observer) => {
      let progress = 0;
      const interval = setInterval(() => {
        progress += 10;
        this.uploadProgress = progress;
        if (progress === 100) {
          clearInterval(interval);
          observer.complete();
        }
      }, 500);
    });
  }
}
