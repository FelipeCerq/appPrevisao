import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../config/api.config';

export interface ClimaAtualResponse {
  cidade?: string;
  temperatura?: number;
  temperaturaMin?: number;
  temperaturaMax?: number;
  umidade?: number;
  descricao?: string;
}

export interface PrevisaoClimaResponse {
  temperaturaMin5dias?: number;
  temperaturaMax5dias?: number;
  dataPrevista?: string;
  descricao?: string;
  iconeTempo?: string;
}

@Injectable({
  providedIn: 'root'
})
export class ClimaService {
  private readonly apiUrl = `${API_BASE_URL}/Clima`;

  constructor(private http: HttpClient) {}

  obterClimaAtual(cidade: string): Observable<ClimaAtualResponse> {
    return this.http.get<ClimaAtualResponse>(
      `${this.apiUrl}/${encodeURIComponent(cidade.trim())}`
    );
  }

  obterPrevisao5Dias(cidade: string): Observable<PrevisaoClimaResponse[]> {
    return this.http.get<PrevisaoClimaResponse[]>(
      `${this.apiUrl}/previsao/${encodeURIComponent(cidade.trim())}`
    );
  }
}
