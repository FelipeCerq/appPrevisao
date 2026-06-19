import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../config/api.config';

export interface CidadeFavoritaResponse {
  id: number;
  nome: string;
  usuarioId: number;
}

@Injectable({
  providedIn: 'root'
})
export class FavoritosService {
  private readonly apiUrl = `${API_BASE_URL}/Favoritos`;

  constructor(private http: HttpClient) {}
  // Não colocar o Usuario ID direto aqui, a API já tem no ClaimsIdentity/JWT, no front é desnecessário e inseguro.  
  listar(): Observable<CidadeFavoritaResponse[]> {
    return this.http.get<CidadeFavoritaResponse[]>(this.apiUrl, {
      headers: this.tokenAcesso()
    });
  }

  adicionar(nomedaCidade: string): Observable<CidadeFavoritaResponse> {
    return this.http.post<CidadeFavoritaResponse>(
      this.apiUrl,
      { nomeCidade: nomedaCidade },
      {
        headers: this.tokenAcesso()
      }
    );
  }

  remover(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`, {
      headers: this.tokenAcesso()
    });
  }

  private tokenAcesso(): HttpHeaders {
    // Salvar o token em local para acessar os favoritos - Importante
    const token = localStorage.getItem('token');
    if (!token) {
      return new HttpHeaders();
    }

    return new HttpHeaders({
      Authorization: `Bearer ${token}`
    });
  }
}
