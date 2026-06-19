import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/services_auth';
import { AuthStateService } from '../../core/services/auth-state.service';
import { ChangeDetectorRef } from '@angular/core';
@Component({
  selector: 'app-cadastro',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './cadastro.html',
  styleUrl: './cadastro.css'
})
export class Cadastro {
  email = '';
  senha = '';
  senha2 = '';
  erro = '';
  carregando = false;
  sucesso = '';

  constructor(
    private authService: AuthService,
    private authState: AuthStateService,
    private router: Router,
    private cdr: ChangeDetectorRef

  ) {}

  cadastrar(): void {
    const email = this.email.trim();
    const senha = this.senha.trim();
    const senha2 = this.senha2.trim();

    const VerificaoEmail = /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(this.email);
    if (!VerificaoEmail) {
      this.erro = 'Não é um e-mail';
      return;
    }

    if (!email || !senha || !senha2) {
      this.erro = 'Preencha todos os campos.';
      return;
    }
    if (senha !== senha2) {
      this.erro = 'As senhas não são iguais.';
      return;
    }

    this.erro = '';
    this.sucesso = '';
    this.carregando = true;

    this.authService.cadastro(email, senha).subscribe({
      next: () => {
        this.authService.login(email, senha).subscribe({
          next: (response) => {
            this.authState.setToken(response.token);
            this.carregando = false;
            this.sucesso = 'Conta criada.';
            this.router.navigate(['/home']);
          },
          error: (erro: unknown) => {
            this.carregando = false;
            this.erro = this.extrairMensagemErro(erro);
            this.cdr.detectChanges();
          }
        });
      },
      error: (erro: unknown) => {
        this.carregando = false;
        this.erro = this.extrairMensagemErro(erro);
        this.cdr.detectChanges();    
      }
    });
  }

  private extrairMensagemErro(erro: unknown): string {
    if (erro instanceof HttpErrorResponse) {
      if (erro.status === 409) {
        return 'Usuário já cadastrado.';
      }
    }
    return 'Erro. Não foi possível cadastrar.';
  }
}
