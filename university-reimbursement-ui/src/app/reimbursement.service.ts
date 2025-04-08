import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ReimbursementService {
  private apiUrl = 'https://localhost:7125/api/reimbursements';

  constructor(private http: HttpClient) {}

  submitReimbursement(formData: FormData): Observable<any> {
    return this.http.post(this.apiUrl, formData);
  }
  getAllReimbursements(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}`);
  }
}
