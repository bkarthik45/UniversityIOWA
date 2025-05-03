import { Component,OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule,FormBuilder,FormGroup,Validators,ReactiveFormsModule } from '@angular/forms';
import { ReimbursementService } from '../reimbursement.service';
import { ToastrService } from 'ngx-toastr';
import { NgForm, AbstractControl } from '@angular/forms';
import { ViewChild, ElementRef } from '@angular/core';


@Component({
  selector: 'app-reimbursement-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule,],
  templateUrl: './reimbursement-form.component.html',
  styleUrls: ['./reimbursement-form.component.css'],
})

export class ReimbursementFormComponent  implements OnInit{
  @ViewChild('fileInput') fileInputRef!: ElementRef<HTMLInputElement>;

  reimbursementForm!: FormGroup;
  selectedFile: File |null = null;
  fileError : Boolean = false;
  message: string = '';
  maxDate: string = '';
  submissionSucces:Boolean = false;
  constructor(
    private fb: FormBuilder,
    private reimbursementService: ReimbursementService,
    private toastr: ToastrService,

  ) {}
  ngOnInit(): void {
    const today = new Date();
    today.setHours(0, 0, 0, 0); // 👈 Prevent UTC time from rolling into the next day
    this.maxDate = today.toISOString().split('T')[0]; // Format: 'yyyy-MM-dd'
  
    this.reimbursementForm = this.fb.group({
      purchaseDate: ['', [Validators.required, this.noFutureDate.bind(this)]],
      amount: ['', [Validators.required, Validators.min(1)]],
      description: ['', [Validators.required, Validators.maxLength(255)]],
      receipt: [null, Validators.required],
    });
  
    
  }
  
  

noFutureDate(control:AbstractControl){
  const inputDate = new Date(control.value);
  const today = new Date();
  today.setHours(0,0,0,0);
  return inputDate >= today ? {futureDate: true}:null;
}

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const file = input.files[0];
      const allowedTypes = ['.pdf', '.jpg', '.jpeg', '.png', '.doc', '.docx'];
      const extension = file.name.toLowerCase().substring(file.name.lastIndexOf('.'));
      
      if (!allowedTypes.includes(extension) || file.size > 5 * 1024 * 1024) {
        this.fileError = true;
        this.selectedFile = null;
        return;
      }
  
      this.fileError = false;
      this.selectedFile = file;
      this.reimbursementForm.patchValue({ receipt: file });

    }
  }
  
  showModal = false;
  reimbursements: any[] = [];

  openModal() {

    this.reimbursementService.getAllReimbursements().subscribe((data) => {
      this.reimbursements = data;
      if(this.reimbursements.length === 0){
        this.toastr.info('No Submissions found')
      }
      this.showModal = true;
    });
  }

  closeModal() {
    this.showModal = false;
  }

  onSubmit(): void {
    if (this.reimbursementForm.invalid || !this.selectedFile) {
      this.reimbursementForm.markAllAsTouched();
      this.fileError = !this.selectedFile;
      return;
    }

  
    const formData = new FormData();
    formData.append('PurchaseDate', this.reimbursementForm.get('purchaseDate')?.value);
    formData.append('Amount', this.reimbursementForm.get('amount')?.value);
    formData.append('description', this.reimbursementForm.get('description')?.value);
    if(this.selectedFile){
      formData.append('File',this.selectedFile)
    }



    this.reimbursementService.submitReimbursement(formData).subscribe({
      next: () => {
          this.toastr.success('Reimbursement submitted successfully!');
          

       
        this.reimbursementForm.reset();

        this.selectedFile = null;
        this.fileError = false;
        if (this.fileInputRef) {
          this.fileInputRef.nativeElement.value = '';
        }
       
      },
      error: () => {
        this.toastr.error('Submission failed. Please try again.');
      },
    });
  }
}
