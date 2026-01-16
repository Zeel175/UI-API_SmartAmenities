import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-attachment',
  templateUrl: './attachment.component.html',
  styleUrl: './attachment.component.scss'
})
export class AttachmentComponent {
  @Input() acceptTypes: string = '*'; // Default to allow all file types
  @Input() multiple: boolean = true; // Allow single or multiple file uploads
  @Input() uploadedFiles: any[] = [];
  @Output() filesChanged = new EventEmitter<File[]>();

  onFileChange(event: any): void {
    const files: File[] = Array.from(event.target.files);
    this.filesChanged.emit(files);
  }

  formatFileSize(size: number): string {
    if (size < 1024) {
      return `${size} B`;
    } else if (size < 1048576) {
      return `${(size / 1024).toFixed(1)} KB`;
    } else {
      return `${(size / 1048576).toFixed(1)} MB`;
    }
  }

  removeAttachment(index: number): void {
    this.uploadedFiles.splice(index, 1); // Remove the file at the given index
  }
  // @Input() multiple: boolean = true; // Allow single or multiple files
  // @Input() acceptedTypes: string = '.pdf,.doc,.docx,.jpg,.jpeg,.png'; // Allowed file types
  // @Output() filesChanged = new EventEmitter<File[]>();
  
  // attachments: { file: File, fileName: string }[] = [];

  // onFileSelected(event: Event): void {
  //   const input = event.target as HTMLInputElement;
  //   if (input.files) {
  //     const filesArray = Array.from(input.files).map(file => ({ file, fileName: file.name }));
  //     this.attachments = this.attachments.concat(filesArray);
  //     this.filesChanged.emit(this.attachments.map(a => a.file));
  //   }
  // }

  // removeAttachment(index: number): void {
  //   this.attachments.splice(index, 1);
  //   this.filesChanged.emit(this.attachments.map(a => a.file));
  // }

  // downloadAttachment(attachment: { file: File, fileName: string }): void {
  //   const url = URL.createObjectURL(attachment.file);
  //   const a = document.createElement('a');
  //   a.href = url;
  //   a.download = attachment.fileName;
  //   document.body.appendChild(a);
  //   a.click();
  //   document.body.removeChild(a);
  //   URL.revokeObjectURL(url);
  // }
}
