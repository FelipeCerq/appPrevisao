import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { AuthStateService } from '../../core/services/auth-state.service';
import { CidadeFavoritaResponse, FavoritosService } from '../../core/services/services_favoritos';

interface FavoritoViewModel extends CidadeFavoritaResponse {
  destaque: boolean;
}

@Component({
  selector: 'app-favoritos',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './favoritos.html',
  styleUrl: './favoritos.css'
})
export class Favoritos implements OnInit {
  cidade = '';
  erro = '';
  mensagem = '';

  private readonly favoritosSubject = new BehaviorSubject<FavoritoViewModel[]>([]);
  readonly favoritos$ = this.favoritosSubject.asObservable();

  constructor(
    private router: Router,
    private favoritosService: FavoritosService,
    private authState: AuthStateService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    if (!this.authState.isAuthenticated()) {
      this.router.navigate(['/login']);
      return;
    }

    this.carregarFavoritos();
  }

  adicionar(): void {
    const nomeCidade = this.cidade.trim();

    if (!nomeCidade) {
      this.erro = 'Informe uma cidade para adicionar.';
      this.mensagem = '';
      return;
    }

    this.erro = '';
    this.mensagem = '';

    this.favoritosService.adicionar(nomeCidade).subscribe({
      next: () => {
        this.cidade = '';
        this.erro = '';
        this.mensagem = 'Cidade adicionada aos favoritos.';
        this.carregarFavoritos();
      },
      error: (error) => {
        console.log(error)
        this.mensagem = '';
        this.erro = error.error;
        this.cdr.detectChanges();
      }
    });
  }

  remover(id: number): void {
    this.erro = '';
    this.mensagem = '';

    this.favoritosService.remover(id).subscribe({
      next: () => {
        this.carregarFavoritos();
      },
      error: () => {
        this.erro = 'Não foi possível remover a cidade.';
      }
    });
  }

  destacar(id: number): void {
    const atualizados = this.favoritosSubject.value.map((item) => ({
      ...item,
      destaque: item.id === id
    }));

    this.favoritosSubject.next(atualizados);
  }

  voltar(): void {
    this.router.navigate(['/home']);
  }

  logout(): void {
    this.authState.clearToken();
    this.router.navigate(['/login']);
  }

  get autenticado(): boolean {
    return this.authState.isAuthenticated();
  }

  atualizaMudou(_: number, item: FavoritoViewModel): number {
    return item.id;
  }

  private carregarFavoritos(): void {
    this.favoritosService.listar().subscribe({
      next: (favoritos) => {
        this.favoritosSubject.next(
          favoritos.map((item) => ({
            ...item,
            destaque: false
          }))
        );
      },
      error: () => {
        this.erro = 'Não foi possível carregar os favoritos.';
      }
    });
  }
}
