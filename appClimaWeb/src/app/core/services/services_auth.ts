import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { API_BASE_URL } from '../config/api.config';

interface LoginResponse {
  token?: string;
  Token?: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  // Recebe a url via API do config
  private readonly apiUrl = `${API_BASE_URL}/authLogin`;

  constructor(private http: HttpClient) { }

  login(email: string, senha: string): Observable<{ token: string }> {
  return this.http.post<{ token: string }>(
    `${this.apiUrl}/login`,
      {
      email: email.trim().toLowerCase(),
      senha: senha.trim()
      }
    );
  }

  cadastro(email: string, senha: string): Observable<{ ok: true }> {    
    return this.http.post<{ ok: true }>(`${this.apiUrl}/cadastro`, 
      {
      email: email.trim().toLowerCase(),
      senha: senha.trim()
    });
  }
}
