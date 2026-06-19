import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/services_auth';
import { AuthStateService } from '../../core/services/auth-state.service';
import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {
  email = '';
  senha = '';
  erro = '';
  carregando = false;

  constructor(
    private authService: AuthService,
    private authState: AuthStateService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}


  login(): void {
    const email = this.email.trim();
    const senha = this.senha.trim();
    
    const VerificaoEmail = /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(this.email);
    if (!VerificaoEmail) {
      this.erro = 'Não é um e-mail';
      return;
    }
    
    if (!email || !senha) {
      this.erro = 'Informe usuário e senha.';
      return;
    }

    this.erro = '';
    this.carregando = true;

    this.authService.login(email, senha).subscribe({
      next: (response) => {
        console.log(response)
        this.authState.setToken(response.token);
        this.carregando = false;
        this.router.navigate(['/home']);
      },
      error: (erro: unknown) => {
        this.carregando = false;
        this.erro = this.extrairMensagemErro(erro);    
        // Cdr é usado para detectar a mudança 
        this.cdr.detectChanges();      
        return this.erro;          
      }
    });
  }

  private extrairMensagemErro(erro: unknown): string {
    if (erro instanceof HttpErrorResponse) {
      if (erro.status === 401) {
        // Manda a mensagem de erro para o usuario via API
        return erro.error;
      }
    }
    return 'Usuário ou senha inválidos.';
  }
}
