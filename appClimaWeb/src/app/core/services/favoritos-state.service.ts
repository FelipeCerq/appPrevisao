import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface FavoritoItem {
  id: number;
  nome: string;
  destaque: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class FavoritosStateService {
  private readonly storageKey = 'appClimaWeb.favoritos';
  private readonly favoritosSubject = new BehaviorSubject<FavoritoItem[]>(this.carregar());

  readonly favoritos$ = this.favoritosSubject.asObservable();

  iniciar(): void {
    this.favoritosSubject.next(this.carregar());
  }

  adicionar(nomeCidade: string): void {
    const cidade = nomeCidade.trim();

    if (!cidade) {
      return;
    }

    const favoritos = this.favoritosSubject.value;

    if (favoritos.some((item) => item.nome.toLowerCase() === cidade.toLowerCase())) {
      return;
    }

    const novoFavorito: FavoritoItem = {
      id: Date.now(),
      nome: cidade,
      destaque: false
    };

    this.salvar([...favoritos, novoFavorito]);
  }

  remover(id: number): void {
    const favoritos = this.favoritosSubject.value.filter((item) => item.id !== id);
    this.salvar(favoritos);
  }

  destacar(id: number): void {
    const favoritos = this.favoritosSubject.value.map((item) => ({
      ...item,
      destaque: item.id === id
    }));

    this.salvar(favoritos);
  }

  private salvar(favoritos: FavoritoItem[]): void {
    this.favoritosSubject.next(favoritos);
    localStorage.setItem(this.storageKey, JSON.stringify(favoritos));
  }

  private carregar(): FavoritoItem[] {
    const raw = localStorage.getItem(this.storageKey);

    if (!raw) {
      return [];
    }

    try {
      const parsed = JSON.parse(raw) as FavoritoItem[];

      if (!Array.isArray(parsed)) {
        return [];
      }

      return parsed
        .filter((item) => !!item?.id && !!item?.nome)
        .map((item) => ({
          id: item.id,
          nome: item.nome,
          destaque: !!item.destaque
        }))
        .sort((a, b) => Number(b.destaque) - Number(a.destaque));
    } catch {
      return [];
    }
  }
}
