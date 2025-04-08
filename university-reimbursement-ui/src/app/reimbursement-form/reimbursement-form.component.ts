import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReimbursementService } from '../reimbursement.service';
import { ToastrService } from 'ngx-toastr';
import { NgForm, AbstractControl } from '@angular/forms';

@Component({
  selector: 'app-reimbursement-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './reimbursement-form.component.html',
  styleUrls: ['./reimbursement-form.component.css'],
})
export class ReimbursementFormComponent {
  formData: any = {
    purchaseDate: '',
    amount: '',
    description: '',
  };

  selectedFile: File | null = null;
  message: string = '';
  fileError: boolean = false;

  constructor(
    private reimbursementService: ReimbursementService,
    private toastr: ToastrService
  ) {}

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
    }
  }
  showModal = false;
  reimbursements: any[] = [];

  openModal() {
    this.reimbursementService.getAllReimbursements().subscribe((data) => {
      this.reimbursements = data;
      this.showModal = true;
    });
  }

  closeModal() {
    this.showModal = false;
  }

  onSubmit(form: NgForm): void {
    if (form.invalid) {
      Object.values(form.controls).forEach((control: AbstractControl) => {
        control.markAsTouched();
      });
      return;
    }

    const purchaseDate = new Date(this.formData.purchaseDate);
    const today = new Date();
    today.setHours(0, 0, 0, 0);

    if (purchaseDate > today) {
      this.toastr.error('Purchase date cannot be in the future.');
      return;
    }

    this.fileError = false;

    if (!this.selectedFile) {
      this.fileError = true;
      return;
    }

    const allowedTypes = ['.pdf', '.jpg', '.jpeg', '.png'];
    const fileName = this.selectedFile.name.toLowerCase();
    const extension = fileName.substring(fileName.lastIndexOf('.'));

    if (!allowedTypes.includes(extension)) {
      this.fileError = true;
      return;
    }

    if (this.selectedFile.size > 5 * 1024 * 1024) {
      this.fileError = true;
      return;
    }

    const formData = new FormData();
    formData.append('purchaseDate', this.formData.purchaseDate);
    formData.append('amount', this.formData.amount);
    formData.append('description', this.formData.description);
    formData.append('file', this.selectedFile);

    this.reimbursementService.submitReimbursement(formData).subscribe({
      next: () => {
        this.toastr.success('Reimbursement submitted successfully!');

        this.formData = {
          purchaseDate: '',
          amount: '',
          description: '',
        };
        this.selectedFile = null;
        this.fileError = false;

        const fileInput = document.getElementById(
          'fileInput'
        ) as HTMLInputElement;
        if (fileInput) fileInput.value = '';

        form.resetForm();
      },
      error: () => {
        this.toastr.error('Submission failed. Please try again.');
      },
    });
  }
}
