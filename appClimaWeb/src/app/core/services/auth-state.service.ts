import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthStateService {
  private readonly tokenKey = 'token';
  private readonly authSubject = new BehaviorSubject<boolean>(this.hasToken());

  readonly isAuthenticated$ = this.authSubject.asObservable();

  isAuthenticated(): boolean {
    return this.hasToken();
  }

  setToken(token: string): void {
    const valor = token.trim();

    if (!valor) {
      this.clearToken();
      return;
    }

    localStorage.setItem(this.tokenKey, valor);
    this.authSubject.next(true);
  }

  clearToken(): void {
    localStorage.removeItem(this.tokenKey);
    this.authSubject.next(false);
  }

  token(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  private hasToken(): boolean {
    return !!localStorage.getItem(this.tokenKey);
  }
}
