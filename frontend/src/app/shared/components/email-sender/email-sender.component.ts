import { Component, EventEmitter, Inject, Input, Output } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { EmailSenderService } from './email-sender.service';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatCardModule } from '@angular/material/card';
//import { EmailService } from '../services/email.service'; // Adjust the path as per your structure

@Component({
  selector: 'app-email-sender',
  standalone: true,
  imports: [
    CommonModule,            // ← add
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatDialogModule, 
    MatCardModule    // ← if your form uses it
    // … any Material modules you use, e.g. MatInputModule, MatButtonModule …
  ],
  templateUrl: './email-sender.component.html',
  styleUrls: ['./email-sender.component.scss'], // Note: Fix `styleUrl` to `styleUrls`
})
export class EmailSenderComponent {
  @Input() receptor!: string;
  @Input() submitLabel = 'Configure';           // NEW
  @Output() formSubmit = new EventEmitter<{
    subject: string; emailAddress?: string; body: string
  }>();

  emailForm: FormGroup;
  formattedEmailBody = '';

  constructor(private fb: FormBuilder) {
    this.emailForm = this.fb.group({
      subject: ['', [Validators.required, Validators.maxLength(100)]],
      // make receptor OPTIONAL (remove required)
      receptor: ['', [Validators.email]],         // CHANGED
      body: ['', [Validators.required]],
    });
  }

  ngOnInit(): void {
    if (this.receptor?.trim()) {
      this.emailForm.patchValue({ receptor: this.receptor });
    }
  }

  submitForm() {
    if (this.emailForm.valid) {
      const v = this.emailForm.value;
      // Emit RAW body (keep tokens) so it can be saved as the template
      this.formSubmit.emit({
        subject: v.subject,
        emailAddress: v.receptor || undefined,
        body: v.body,                              // CHANGED: not formatted here
      });
      this.emailForm.reset();
    }
  }

  updatePreview() {
    const body = this.emailForm.get('body')?.value || '';
    this.formattedEmailBody = body.replace(/\n/g, '<br>');
  }
}
