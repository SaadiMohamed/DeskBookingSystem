import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LoginService {

  private apiUrl = 'https://localhost:7161/api/AuthManagement';

  constructor(private http: HttpClient) { }

  login(email: string, password: string): Observable<Authresult> {
    const credentials = { email, password };
    console.log(credentials)
    return this.http.post<Authresult>(this.apiUrl + '/Login', credentials)
  }
}

export interface Authresult {
  token: string
  result: boolean
  errors: string[]
}


